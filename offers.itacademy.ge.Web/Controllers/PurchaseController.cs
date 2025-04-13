using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using offers.itacademy.ge.Application.Interfaces.ServiceInterfaces;
using offers.itacademy.ge.Domain.Users;
using System.Security.Claims;
using System.Threading;

namespace offers.itacademy.ge.Web.Controllers
{
    public class PurchaseController : Controller
    {
        private readonly IPurchaseService _purchaseService;
        private readonly IProductOfferService _productOfferService;

        public PurchaseController(IPurchaseService purchaseService, IProductOfferService productOfferService)
        {
            _purchaseService = purchaseService;
            _productOfferService = productOfferService;
        }

        // GET: /Purchase/Buy/{offerId} (Customer)
        [Authorize(Roles = "Customer")]
        [HttpGet]
        public async Task<IActionResult> Buy(int offerId, CancellationToken token = default)
        {
            var offer = await _productOfferService.GetOfferByIdAsync(offerId, token);
            if (offer == null || offer.Status != offers.itacademy.ge.Domain.ProductOffers.OfferStatus.Active)
            {
                TempData["Error"] = "Offer not found or not active.";
                return RedirectToAction("ActiveOffers", "ProductOffer");
            }
            return View(offer);
        }

        // POST: /Purchase/Buy/{offerId} (Customer)
        [Authorize(Roles = "Customer")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Buy(int offerId, int quantity, CancellationToken token = default)
        {
            if (quantity <= 0)
            {
                ModelState.AddModelError("", "Quantity must be greater than zero.");
                var offer = await _productOfferService.GetOfferByIdAsync(offerId, token);
                return View(offer);
            }

            try
            {
                var userId = int.Parse(User.FindFirstValue("UserId"));
                var result = await _purchaseService.PurchaseOfferAsync(userId, offerId, quantity, token);
                TempData["Success"] = "Purchase completed successfully!";
                return RedirectToAction("MyPurchases");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                var offer = await _productOfferService.GetOfferByIdAsync(offerId, token);
                return View(offer);
            }
        }

        // GET: /Purchase/Cancel/{purchaseId} (Customer)
        [Authorize(Roles = "Customer")]
        [HttpGet]
        public async Task<IActionResult> Cancel(int purchaseId, CancellationToken token = default)
        {
            var userId = int.Parse(User.FindFirstValue("UserId"));
            var purchase = await _purchaseService.GetPurchaseByIdAsync(purchaseId, userId, token);
            if (purchase == null || purchase.CustomerId != userId)
            {
                TempData["Error"] = "Purchase not found or you don’t have permission to cancel it.";
                return RedirectToAction("MyPurchases");
            }
            return View(purchase);
        }

        // POST: /Purchase/CancelConfirmed/{purchaseId} (Customer)
        [Authorize(Roles = "Customer")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelConfirmed(int purchaseId, CancellationToken token = default)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue("UserId"));
                var success = await _purchaseService.CancelPurchaseAsync(purchaseId, userId, token);
                if (success)
                    TempData["Success"] = "Purchase cancelled successfully!";
                else
                    TempData["Error"] = "Purchase cannot be cancelled (e.g., past time limit).";
                return RedirectToAction("MyPurchases");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Cancel", new { purchaseId });
            }
        }

        // GET: /Purchase/MyPurchases (Customer/Company)
        [Authorize(Roles = "Customer,Company")]
        [HttpGet]
        public async Task<IActionResult> MyPurchases(int pageNumber = 1, int pageSize = 10, CancellationToken token = default)
        {
            var userId = int.Parse(User.FindFirstValue("UserId"));
            var purchases = await _purchaseService.GetUserPurchasesAsync(pageNumber, pageSize, userId, token);
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            return View(purchases);
        }

        // GET: /Purchase/UserPurchases?userId={userId} (Admin)
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> UserPurchases(int userId, int pageNumber = 1, int pageSize = 10, CancellationToken token = default)
        {
            try
            {
                var purchases = await _purchaseService.GetUserPurchasesAsync(pageNumber, pageSize, userId, token);
                if (purchases == null || !purchases.Any())
                {
                    TempData["Error"] = $"No purchases found for user ID {userId}.";
                    return RedirectToAction("Profile", "Admin");
                }
                ViewBag.UserId = userId;
                ViewBag.PageNumber = pageNumber;
                ViewBag.PageSize = pageSize;
                return View(purchases);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error retrieving purchases for user ID {userId}: {ex.Message}";
                return RedirectToAction("Profile", "Admin");
            }
        }

        // GET: /Purchase/OfferPurchases?offerId={offerId} (Company/Admin)
        [Authorize(Roles = "Company,Admin")]
        [HttpGet]
        public async Task<IActionResult> OfferPurchases(int offerId, int pageNumber = 1, int pageSize = 10, CancellationToken token = default)
        {
            try
            {
                var requestingUserId = int.Parse(User.FindFirstValue("UserId"));
                var requestingUserRole = Enum.Parse<UserRole>(User.FindFirstValue(ClaimTypes.Role));
                var purchases = await _purchaseService.GetOfferPurchasesAsync(pageNumber, pageSize, offerId, requestingUserId, requestingUserRole, token);
                if (purchases == null || !purchases.Any())
                {
                    TempData["Error"] = $"No purchases found for offer ID {offerId}.";
                    return RedirectToAction("Profile", "Admin");
                }
                ViewBag.OfferId = offerId;
                ViewBag.PageNumber = pageNumber;
                ViewBag.PageSize = pageSize;
                return View(purchases);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error retrieving purchases for offer ID {offerId}: {ex.Message}";
                return RedirectToAction("Profile", "Admin");
            }
        }
    }
}