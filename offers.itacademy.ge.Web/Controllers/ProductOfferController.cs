using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using offers.itacademy.ge.Application.Interfaces.ServiceInterfaces;
using offers.itacademy.ge.Application.Models.Images;
using offers.itacademy.ge.Application.Models.ProductOffers;
using System.Security.Claims;
using System.Threading;

namespace offers.itacademy.ge.Web.Controllers
{
    [Authorize]
    public class ProductOfferController : Controller
    {
        private readonly IProductOfferService _productOfferService;

        public ProductOfferController(IProductOfferService productOfferService)
        {
            _productOfferService = productOfferService;
        }

        // GET: /ProductOffer/MyOffers (Company only)
        [Authorize(Roles = "Company")]
        [HttpGet]
        public async Task<IActionResult> MyOffers(int pageNumber = 1, int pageSize = 10, CancellationToken token = default)
        {
            var companyId = int.Parse(User.FindFirstValue("UserId"));
            var offers = await _productOfferService.GetCompanyOffersAsync(pageNumber, pageSize, companyId, token);
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            return View(offers);
        }

        // GET: /ProductOffer/Create (Company only)
        [Authorize(Roles = "Company")]
        [HttpGet]
        public IActionResult Create()
        {
            return View(new ProductOfferRequestModel());
        }

        // POST: /ProductOffer/Create (Company only)
        [Authorize(Roles = "Company")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductOfferRequestModel model, IFormFile? imageFile, CancellationToken token = default)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var companyId = int.Parse(User.FindFirstValue("UserId"));
                var offer = await _productOfferService.CreateOfferAsync(model, companyId, token);

                if (imageFile != null && imageFile.Length > 0)
                {
                    var imageModel = new ImageRequestModel { File = imageFile };
                    await _productOfferService.UploadProductOfferImage(companyId, offer.Id, imageModel, token);
                }

                TempData["Success"] = "Product offer created successfully!";
                return RedirectToAction(nameof(MyOffers));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Failed to create offer: {ex.Message}");
                return View(model);
            }
        }

        // GET: /ProductOffer/Delete/{id} (Company only)
        [Authorize(Roles = "Company")]
        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken token = default)
        {
            var companyId = int.Parse(User.FindFirstValue("UserId"));
            var offer = await _productOfferService.GetOfferByIdAsync(id, token);
            if (offer == null || offer.CompanyId != companyId)
                return NotFound();
            return View(offer);
        }

        // POST: /ProductOffer/DeleteConfirmed (Company only)
        [Authorize(Roles = "Company")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken token = default)
        {
            try
            {
                var companyId = int.Parse(User.FindFirstValue("UserId"));
                await _productOfferService.DeleteOfferAsync(id, companyId, token);
                TempData["Success"] = "Product offer deleted successfully!";
                return RedirectToAction(nameof(MyOffers));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Delete), new { id });
            }
        }

        // GET: /ProductOffer/Cancel/{id} (Company only)
        [Authorize(Roles = "Company")]
        [HttpGet]
        public async Task<IActionResult> Cancel(int id, CancellationToken token = default)
        {
            var companyId = int.Parse(User.FindFirstValue("UserId"));
            var offer = await _productOfferService.GetOfferByIdAsync(id, token);
            if (offer == null || offer.CompanyId != companyId)
                return NotFound();
            return View(offer);
        }

        // POST: /ProductOffer/CancelConfirmed (Company only)
        [Authorize(Roles = "Company")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelConfirmed(int id, CancellationToken token = default)
        {
            try
            {
                var companyId = int.Parse(User.FindFirstValue("UserId"));
                var success = await _productOfferService.CancelOfferAsync(id, companyId, token);
                if (success)
                    TempData["Success"] = "Product offer cancelled successfully!";
                else
                    TempData["Error"] = "Offer cannot be cancelled (e.g., past time limit).";
                return RedirectToAction(nameof(MyOffers));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Cancel), new { id });
            }
        }

        // GET: /ProductOffer/Edit/{id} (Company only)
        [Authorize(Roles = "Company")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken token = default)
        {
            var companyId = int.Parse(User.FindFirstValue("UserId"));
            var offer = await _productOfferService.GetOfferByIdAsync(id, token);
            if (offer == null || offer.CompanyId != companyId)
                return NotFound();

            var model = new ProductOfferRequestModel
            {
                Name = offer.Name,
                Description = offer.Description,
                Price = offer.Price,
                Quantity = offer.Quantity,
                ExpirationTime = offer.ExpirationTime,
                CategoryId = offer.CategoryId
            };
            ViewBag.ImageUrl = offer.ImageUrl;
            return View(model);
        }

        // POST: /ProductOffer/Edit/{id} (Company only)
        [Authorize(Roles = "Company")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductOfferRequestModel model, IFormFile? imageFile, CancellationToken token = default)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var companyId = int.Parse(User.FindFirstValue("UserId"));
                await _productOfferService.UpdateOfferAsync(id, model, companyId, token);

                if (imageFile != null && imageFile.Length > 0)
                {
                    var imageModel = new ImageRequestModel { File = imageFile };
                    await _productOfferService.UploadProductOfferImage(companyId, id, imageModel, token);
                }

                TempData["Success"] = "Product offer updated successfully!";
                return RedirectToAction(nameof(MyOffers));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Failed to update offer: {ex.Message}");
                return View(model);
            }
        }

        // GET: /ProductOffer/GetAll (Admin only)
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAll(int pageNumber = 1, int pageSize = 10, CancellationToken token = default)
        {
            var offers = await _productOfferService.GetAllOffersAsync(pageNumber, pageSize, token);
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            return View(offers);
        }

        // GET: /ProductOffer/ActiveOffers (Everyone)
        [HttpGet]
        public async Task<IActionResult> ActiveOffers(int pageNumber = 1, int pageSize = 10, CancellationToken token = default)
        {
            if (User.IsInRole("Customer"))
            {
                return RedirectToAction(nameof(RelevantOffers), new { pageNumber, pageSize });
            }

            var offers = await _productOfferService.GetAllActiveOffersAsync(pageNumber, pageSize, token);
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            return View(offers);
        }

        // GET: /ProductOffer/RelevantOffers (Customer only)
        [Authorize(Roles = "Customer")]
        [HttpGet]
        public async Task<IActionResult> RelevantOffers(int pageNumber = 1, int pageSize = 10, CancellationToken token = default)
        {
            var customerId = int.Parse(User.FindFirstValue("UserId"));
            var offers = await _productOfferService.GetUserRelevantOffersAsync(pageNumber, pageSize, customerId, token);
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            return View(offers);
        }
    }
}