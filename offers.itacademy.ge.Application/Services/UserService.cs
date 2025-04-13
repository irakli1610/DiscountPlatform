using Mapster;
using Microsoft.Extensions.Options;
using offers.itacademy.ge.Application.Exceptions;
using offers.itacademy.ge.Application.Exceptions.Resources;
using offers.itacademy.ge.Application.Interfaces;
using offers.itacademy.ge.Application.Interfaces.ServiceInterfaces;
using offers.itacademy.ge.Application.Models.Images;
using offers.itacademy.ge.Application.Models.Users;
using offers.itacademy.ge.Application.Models.Users.Admin;
using offers.itacademy.ge.Application.Models.Users.Company;
using offers.itacademy.ge.Application.Models.Users.Customer;
using offers.itacademy.ge.Application.Services.ImageServices;
using offers.itacademy.ge.Application.Utils;
using offers.itacademy.ge.Application.Utils.Auth.JWT;
using offers.itacademy.ge.Domain.Users;

namespace offers.itacademy.ge.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOptions<JWTConfiguration> _jwtOptions;
        private readonly IOptions<ImageUploadSettings> _imageOptions;

        private readonly IFileService _fileService;
        public UserService(IUnitOfWork unitOfWork, IOptions<JWTConfiguration> jwtOptions, IFileService fileService, IOptions<ImageUploadSettings> imageOptions)
        {
            _unitOfWork = unitOfWork;
            _jwtOptions = jwtOptions;
            _fileService = fileService;
            _imageOptions = imageOptions;
        }


        public async Task<List<TResponse>> GetUsersAsync<TUserEntity, TResponse>(int pageNumber, int pageSize, CancellationToken token)
              where TUserEntity : User
              where TResponse : UserResponseModel
        {
            ValidationUtils.ValidatePagination(pageNumber, pageSize);

            var entities = await _unitOfWork.Users.GetUsersAsync<TUserEntity>(pageNumber, pageSize, token);
            return entities.Adapt<List<TResponse>>();
        }

        public async Task<bool> ActivateCompanyAsync(int companyId, CancellationToken token)
        {
            var company = await _unitOfWork.Users.GetByIdAsync(companyId, token) as Company
                ?? throw new NotFoundException(AppExceptionMessages.CompanyNotFound);

            if (company.IsActivated)
            {
                throw new InvalidOperationException(AppExceptionMessages.CompanyAlreadyActivated);
            }

            company.IsActivated = true;
            _unitOfWork.Users.Update(company);
            await _unitOfWork.SaveChangesAsync(token);
            return true;
        }

        public async Task<bool> UpdateCustomerCategoriesAsync(int customerId, List<int> categoryIds, CancellationToken token)
        {
            var customer = await _unitOfWork.Users.GetUserWithCategoriesAsync(customerId, token) as Customer
                ?? throw new NotFoundException(AppExceptionMessages.CustomerNotFound);

            var categories = await _unitOfWork.Categories.GetByIdsAsync(categoryIds, token);
            if (categories.Count != categoryIds.Count)
                throw new NotFoundException(AppExceptionMessages.CategoryNotFound);

            customer.SelectedCategories.Clear();
            customer.SelectedCategories.AddRange(categories);

            _unitOfWork.Users.Update(customer);
            await _unitOfWork.SaveChangesAsync(token);
            return true;
        }

        public async Task<string> AuthenticateAsync(UserLoginRequestModel model, CancellationToken token)
        {
            var user = await _unitOfWork.Users.GetUserByUserName(x => x.UserName == model.UserName, token)
                ?? throw new UnauthorizedAccessException(AppExceptionMessages.UserOrPasswordNotValid);

            // Verify the password
            if (!AuthUtils.VerifyPassword(model.Password, user.Password))
            {
                throw new UnauthorizedAccessException(AppExceptionMessages.UserOrPasswordNotValid);
            }

            // Generate JWT token for the user
            var jwtToken = JwtHelper.GenerateToken(user, _jwtOptions);

            return jwtToken;
        }

        public async Task<TResponse> RegisterAsync<TRequest, TResponse>(TRequest model, CancellationToken token)
             where TRequest : PasswordRequestModel
             where TResponse : UserResponseModel
        {
            var exists = await _unitOfWork.Users.ExistsAsync(x => x.UserName == model.UserName, token);
            if (exists)
            {
                throw new ObjectAlreadyExistsException(AppExceptionMessages.UserAlreadyExists);
            }

            User user = GenerateUser(model);

            user.Password = AuthUtils.GeneratePasswordHash(model.Password);
            user.Role = user switch
            {
                Admin => UserRole.Admin,
                Company => UserRole.Company,
                Customer => UserRole.Customer,
                _ => throw new ArgumentException(AppExceptionMessages.InvalidUserType)
            };

            //  Set default image if user is a Company
            if (user is Company company)
            {
                company.ImageUrl = _imageOptions.Value.DefaultCompanyImageName;
            }

            var result = await _unitOfWork.Users.AddAsync(user, token);
            await _unitOfWork.SaveChangesAsync(token);

            return result.Adapt<TResponse>(); // return proper model

        }
       
        public async Task<TResponse> UpdateUserAsync<TRequest, TResponse>(int userId, TRequest model, CancellationToken token)
          where TRequest : UserRequestModel
          where TResponse : UserResponseModel
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId, token);
            if (user == null)
            {
                throw new NotFoundException(AppExceptionMessages.UserNotFound);
            }

            // Update common fields
            user.UserName = model.UserName;
            user.Email = model.Email;

            // Handle user-specific updates for different user types
            if (user is Company company && model is CompanyRequestUpdateModel companyModel)
                company.Balance = companyModel.Balance;
            else if (user is Customer customer && model is CustomerRequestUpdateModel customerModel)
                customer.Balance = customerModel.Balance;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync(token);

            return user.Adapt<TResponse>();
        }
       
        public async Task<string> UploadCompanyImage(int companyId, ImageRequestModel fileForm, CancellationToken token)
        {
            var company = await _unitOfWork.Users.GetByIdAsync(companyId, token) as Company
                ?? throw new NotFoundException(AppExceptionMessages.CompanyNotFound);

            string uploadedFileName = string.Empty;
            string? oldFileName = company.ImageUrl; // Store only file name

            try
            {
                uploadedFileName = await _fileService.UploadImageAsync(fileForm.File!, _imageOptions.Value.CompanyImagePath, token);

                company.ImageUrl = uploadedFileName; // Store only the file name
                _unitOfWork.Users.Update(company);

                await _unitOfWork.SaveChangesAsync(token);

                if (!string.IsNullOrEmpty(oldFileName))
                {
                    await _fileService.DeleteImageAsync(oldFileName, _imageOptions.Value.CompanyImagePath, token);
                }

                return company.ImageUrl;
            }
            catch
            {
                // If failure occurs after upload, delete the uploaded file
                if (!string.IsNullOrEmpty(uploadedFileName))
                {
                    await _fileService.DeleteImageAsync(uploadedFileName, _imageOptions.Value.CompanyImagePath, token);
                }

                throw;
            }
        }

        public async Task<UserResponseModel> GetByIdAsync(int userId, CancellationToken token)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId, token)
                ?? throw new NotFoundException(AppExceptionMessages.UserNotFound);
            return user switch
            {
                Customer customer => customer.Adapt<CustomerResponseModel>(),
                Company company => company.Adapt<CompanyResponseModel>(),
                Admin admin => admin.Adapt<AdminResponseModel>(),
                _ => throw new ArgumentException(AppExceptionMessages.InvalidUserType)
            };
        }

        public async Task<Customer> GetCustomerWithCategories(int customerId, CancellationToken token)
        {
            return await _unitOfWork.Users.GetUserWithCategoriesAsync(customerId, token) as Customer
                ?? throw new NotFoundException(AppExceptionMessages.CustomerNotFound);
        }

        private static User GenerateUser(UserRequestModel model)
        {
            return model switch
            {
                AdminRequestModel admin => admin.Adapt<Admin>(),
                CompanyRequestModel company => company.Adapt<Company>(),
                CustomerRequestModel customer => customer.Adapt<Customer>(),
                _ => throw new ArgumentException(AppExceptionMessages.InvalidUserType)
            };
        }
    }
}
