using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using offers.itacademy.ge.Application.Interfaces.ServiceInterfaces;
using offers.itacademy.ge.Application.Models.Users.Customer;
using offers.itacademy.ge.Web.Models;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace offers.itacademy.ge.Web.Controllers
{
    [Authorize(Roles = "Customer")]
    public class CustomerController : Controller
    {
        private readonly IUserService _userService;
        private readonly ICategoryService _categoryService;

        public CustomerController(IUserService userService, ICategoryService categoryService)
        {
            _userService = userService;
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Profile(CancellationToken token = default)
        {
            var customerId = int.Parse(User.FindFirstValue("UserId"));
            var customer = await _userService.GetByIdAsync(customerId, token);
            if (customer is not CustomerResponseModel customerModel)
                return BadRequest("User is not a customer.");
            return View(customerModel);
        }

        [HttpGet]
        public async Task<IActionResult> ManageSubscriptions(CancellationToken token = default)
        {
            var customerId = int.Parse(User.FindFirstValue("UserId"));
            var customer = await _userService.GetCustomerWithCategories(customerId, token);
            if (customer == null) return NotFound();

            var allCategories = await _categoryService.GetAllCategoriesAsync(1, int.MaxValue, token);
            var subscribedCategoryIds = customer.SelectedCategories?.Select(c => c.Id).ToList() ?? new List<int>();

            var model = new CustomerSubscriptionsViewModel
            {
                AvailableCategories = allCategories,
                SubscribedCategoryIds = subscribedCategoryIds
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageSubscriptions([Bind("SubscribedCategoryIds")] CustomerSubscriptionsViewModel model, CancellationToken token = default)
        {
            try
            {
                var customerId = int.Parse(User.FindFirstValue("UserId"));
                var success = await _userService.UpdateCustomerCategoriesAsync(customerId, model.SubscribedCategoryIds ?? new List<int>(), token);
                if (success)
                {
                    TempData["Success"] = "Subscriptions updated successfully!";
                    return RedirectToAction("Profile");
                }
                ModelState.AddModelError("", "Failed to update subscriptions.");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }

            model.AvailableCategories = await _categoryService.GetAllCategoriesAsync(1, int.MaxValue, token);
            return View(model);
        }
    }
}