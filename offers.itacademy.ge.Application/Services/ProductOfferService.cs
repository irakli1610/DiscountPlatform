using Mapster;
using Microsoft.Extensions.Options;
using offers.itacademy.ge.Application.Exceptions;
using offers.itacademy.ge.Application.Exceptions.Resources;
using offers.itacademy.ge.Application.Interfaces;
using offers.itacademy.ge.Application.Interfaces.ServiceInterfaces;
using offers.itacademy.ge.Application.Models.Images;
using offers.itacademy.ge.Application.Models.ProductOffers;
using offers.itacademy.ge.Application.Services.ImageServices;
using offers.itacademy.ge.Application.Utils;
using offers.itacademy.ge.Domain.ProductOffers;
using offers.itacademy.ge.Domain.Purchases;
using offers.itacademy.ge.Domain.Users;
using System.Data;

namespace offers.itacademy.ge.Application.Services;

public class ProductOfferService : IProductOfferService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOptions<ImageUploadSettings> _options;
    private readonly IFileService _fileService;

    public ProductOfferService(IUnitOfWork unitOfWork, IFileService fileService, IOptions<ImageUploadSettings> options)
    {
        _unitOfWork = unitOfWork;
        _fileService = fileService;
        _options = options;
    }

    public async Task<List<ProductOfferResponseModel>> GetAllOffersAsync(int pageNumber, int pageSize, CancellationToken token)
    {
        ValidationUtils.ValidatePagination(pageNumber, pageSize);

        var productOffers = await _unitOfWork.ProductOffers.GetAllAsyncIncludeDetails(pageNumber, pageSize, token);
        var result = productOffers.Adapt<List<ProductOfferResponseModel>>();
        return result;
    }

    public async Task<List<ProductOfferResponseModel>> GetAllActiveOffersAsync(int pageNumber, int pageSize, CancellationToken token)
    {
        ValidationUtils.ValidatePagination(pageNumber, pageSize);

        var productOffers = await _unitOfWork.ProductOffers.GetActiveOffersAsync(pageNumber, pageSize, token);
        return productOffers.Adapt<List<ProductOfferResponseModel>>();
    }

    public async Task<ProductOfferResponseModel> GetOfferByIdAsync(int id, CancellationToken token)
    {
        var productOffer = await _unitOfWork.ProductOffers.GetByIdAsyncIncludingDetails(id, token)
            ?? throw new NotFoundException(AppExceptionMessages.ProductNotFound);

        return productOffer.Adapt<ProductOfferResponseModel>();
    }

    public async Task<ProductOfferResponseModel> CreateOfferAsync(ProductOfferRequestModel offer, int companyId, CancellationToken token)
    {
        var company = await _unitOfWork.Users.GetByIdAsync(companyId, token) as Company
            ?? throw new NotFoundException(AppExceptionMessages.CompanyNotFound);

        if (!company.IsActivated)
        {
            throw new InvalidOperationException(AppExceptionMessages.CompanyNotActivated);
        }

        var isCategoryValid = await _unitOfWork.Categories.ExistsAsync(x => x.Id == offer.CategoryId, token);
        if (!isCategoryValid)
        {
            throw new NotFoundException(AppExceptionMessages.CategoryNotFound);
        }

        var productOffer = offer.Adapt<ProductOffer>();

        productOffer.CompanyId = companyId;
        productOffer.Status = OfferStatus.Active;
        productOffer.CreateTime = DateTime.UtcNow;  // fixed with tests
        productOffer.ImageUrl = _options.Value.DefaultOfferImageName;

        await _unitOfWork.ProductOffers.AddAsync(productOffer, token);
        await _unitOfWork.SaveChangesAsync(token);
        var result = productOffer.Adapt<ProductOfferResponseModel>();
        return result;
    }

    public async Task DeleteOfferAsync(int offerId, int companyId, CancellationToken token)
    {
        var ProductOffer = await _unitOfWork.ProductOffers.GetByIdAsync(offerId, token)
            ?? throw new NotFoundException(AppExceptionMessages.ProductNotFound);

        if (ProductOffer.CompanyId != companyId)
            throw new UnauthorizedAccessException(AppExceptionMessages.UnauthorizedOfferModification);

        _unitOfWork.ProductOffers.Remove(ProductOffer);
        await _unitOfWork.SaveChangesAsync(token);
    }

    public async Task<ProductOfferResponseModel> UpdateOfferAsync(int productOfferId, ProductOfferRequestModel updatedOffer, int companyId, CancellationToken token)
    {
        var productOffer = await _unitOfWork.ProductOffers.GetByIdAsync(productOfferId, token);

        if (!await _unitOfWork.Categories.ExistsAsync(x => x.Id == updatedOffer.CategoryId, token))
        {
            throw new NotFoundException(AppExceptionMessages.CategoryNotFound);
        }

        ValidationUtils.ValidateProductOfferForUpdate(productOffer, companyId);

        productOffer = updatedOffer.Adapt(productOffer);
        _unitOfWork.ProductOffers.Update(productOffer!);
        await _unitOfWork.SaveChangesAsync(token);

        return productOffer.Adapt<ProductOfferResponseModel>();
    }

    public async Task<bool> CancelOfferAsync(int offerId, int companyId, CancellationToken token)
    {
        await using var transaction = await _unitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted, token);
        try
        {
            var offer = await _unitOfWork.ProductOffers.GetCancellableOfferAsync(offerId, companyId, token);

            ValidationUtils.ValidateOfferForCancel(offer);

            // Process refunds
            foreach (var purchase in offer!.Purchases.Where(p => p.Status == PurchaseStatus.Active))
            {
                // 2. Validate purchase elements
                if (purchase.Customer == null)
                {
                    throw new InvalidOperationException(AppExceptionMessages.InvalidPurchaseState);
                }

                var customer = purchase.Customer;
                decimal refundAmount = purchase.TotalPrice;

                // 3. Validate refund amount
                if (offer.Company.Balance < refundAmount)
                {
                    throw new InvalidOperationException(AppExceptionMessages.InsufficientCompanyBalance);
                }

                // 4. Update balances
                customer!.Balance += refundAmount;
                offer.Company.Balance -= refundAmount;

                // 5. Update purchase status and Quantity
                purchase.Status = PurchaseStatus.Cancelled;
                offer.Quantity += purchase.Quantity;

                // 6. Update entities
                _unitOfWork.Users.Update(customer);
                _unitOfWork.Purchases.Update(purchase);
                _unitOfWork.Users.Update(offer.Company); // ბალანსი გარეთ გამოვიტანო? შეამოწმე
            }

            // 7. Update offer status
            offer.Status = OfferStatus.Cancelled;

            _unitOfWork.ProductOffers.Update(offer);
            await _unitOfWork.SaveChangesAsync(token);
            await transaction.CommitAsync(token);
            return true;

        }
        catch
        {
            await transaction.RollbackAsync(token);
            throw;
        }
    }

    public async Task<List<ProductOfferResponseModel>> GetUserRelevantOffersAsync(int pageNumber, int pageSize, int userId, CancellationToken token)
    {
        ValidationUtils.ValidatePagination(pageNumber, pageSize);

        var customer = await _unitOfWork.Users.GetUserWithCategoriesAsync(userId, token) as Customer
            ?? throw new NotFoundException(AppExceptionMessages.CustomerNotFound);

        var relevantOffers = await _unitOfWork.ProductOffers.GetActiveOffersByCategoriesAsync(pageNumber, pageSize, customer.SelectedCategories, token);
        return relevantOffers.Adapt<List<ProductOfferResponseModel>>();
    }

    public async Task<List<ProductOfferResponseModel>> GetCompanyOffersAsync(int pageNumber, int pageSize, int companyId, CancellationToken token)
    {
        ValidationUtils.ValidatePagination(pageNumber, pageSize);
        if(!await _unitOfWork.Users.ExistsAsync(x=>x.Id == companyId, token))
            throw new NotFoundException(AppExceptionMessages.CompanyNotFound);

        var productOffers = await _unitOfWork.ProductOffers.GetCompanyOffersAsync(pageNumber, pageSize, companyId, token);
        return productOffers.Adapt<List<ProductOfferResponseModel>>();
    }

    public async Task<string> UploadProductOfferImage(int companyId, int productOfferId, ImageRequestModel fileForm, CancellationToken token)
    {
        var company = await _unitOfWork.Users.GetByIdAsync(companyId, token) as Company
            ?? throw new NotFoundException(AppExceptionMessages.CompanyNotFound);

        var productOffer = await _unitOfWork.ProductOffers.GetByIdAsync(productOfferId, token)
            ?? throw new NotFoundException(AppExceptionMessages.ProductNotFound);

        if (productOffer.CompanyId != company.Id)
        {
            throw new UnauthorizedAccessException(AppExceptionMessages.UnauthorizedAccess);
        }

        string uploadedFileName = string.Empty;
        string? oldFileName = productOffer.ImageUrl; // Store only file name

        try
        {
            uploadedFileName = await _fileService.UploadImageAsync(fileForm.File!, _options.Value.OfferImagePath, token);

            productOffer.ImageUrl = uploadedFileName;
            _unitOfWork.ProductOffers.Update(productOffer);

            await _unitOfWork.SaveChangesAsync(token);

            if (!string.IsNullOrEmpty(oldFileName))
            {
                await _fileService.DeleteImageAsync(oldFileName, _options.Value.OfferImagePath, token);
            }
            return productOffer.ImageUrl;
        }
        catch
        {
            // If a failure occurs after upload, delete the uploaded file to avoid orphaned images
            if (!string.IsNullOrEmpty(uploadedFileName))
            {
                await _fileService.DeleteImageAsync(uploadedFileName, _options.Value.OfferImagePath, token);
            }
            throw;
        }
    }
}
