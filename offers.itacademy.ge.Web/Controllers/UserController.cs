using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using offers.itacademy.ge.Application.Interfaces.ServiceInterfaces;
using offers.itacademy.ge.Application.Models.Images;
using offers.itacademy.ge.Application.Models.Users;
using offers.itacademy.ge.Application.Models.Users.Company;
using offers.itacademy.ge.Application.Models.Users.Customer;
using System.Security.Claims;
using System.Threading;
using Microsoft.Extensions.Logging;
using offers.itacademy.ge.Domain.Users;
using offers.itacademy.ge.Application.Models.Users.Admin;

namespace offers.itacademy.ge.Web.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger = null)
        {
            _userService = userService;
            _logger = logger;
        }

        // GET: /User/UpdateProfile (Customer/Company)
        [Authorize(Roles = "Customer,Company")]
        [HttpGet]
        public async Task<IActionResult> UpdateProfile(CancellationToken token = default)
        {
            var userId = int.Parse(User.FindFirstValue("UserId"));
            var user = await _userService.GetByIdAsync(userId, token);
            if (user == null) return NotFound();

            if (User.IsInRole("Customer"))
            {
                return View("UpdateCustomer", user.Adapt<CustomerRequestUpdateModel>());
            }
            else // Company
            {
                if (user is not CompanyResponseModel company)
                    return BadRequest("User is not a company.");
                ViewBag.ImageUrl = company.ImageUrl;
                return View("UpdateCompany", company.Adapt<CompanyRequestUpdateModel>());
            }
        }

        // POST: /User/UpdateProfile (Customer)
        [Authorize(Roles = "Customer")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCustomer(CustomerRequestUpdateModel model, CancellationToken token = default)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Please correct the errors below.");
                return View("UpdateCustomer", model);
            }

            try
            {
                var userId = int.Parse(User.FindFirstValue("UserId"));
                await _userService.UpdateUserAsync<CustomerRequestUpdateModel, CustomerResponseModel>(userId, model, token);
                TempData["Success"] = "Profile updated successfully!";
                return RedirectToAction("Profile", "Customer");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to update customer profile for user {UserId}", User.FindFirstValue("UserId"));
                ModelState.AddModelError("", $"Error updating profile: {ex.Message}");
                return View("UpdateCustomer", model);
            }
        }

        // POST: /User/UpdateProfile (Company)
        [Authorize(Roles = "Company")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCompany(CompanyRequestUpdateModel model, CancellationToken token = default)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Please correct the errors below.");
                return View("UpdateCompany", model);
            }

            try
            {
                var userId = int.Parse(User.FindFirstValue("UserId"));
                await _userService.UpdateUserAsync<CompanyRequestUpdateModel, CompanyResponseModel>(userId, model, token);
                TempData["Success"] = "Profile updated successfully!";
                return RedirectToAction("Profile", "Company");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to update company profile for user {UserId}", User.FindFirstValue("UserId"));
                ModelState.AddModelError("", $"Error updating profile: {ex.Message}");
                return View("UpdateCompany", model);
            }
        }

        // POST: /User/UploadCompanyImage
        [Authorize(Roles = "Company")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadCompanyImage(ImageRequestModel model, CancellationToken token = default)
        {
            if (model.File == null || model.File.Length == 0)
            {
                TempData["Error"] = "Please select an image to upload.";
                return RedirectToAction("Profile", "Company");
            }

            try
            {
                var companyId = int.Parse(User.FindFirstValue("UserId"));
                var imageUrl = await _userService.UploadCompanyImage(companyId, model, token);
                TempData["Success"] = "Company image uploaded successfully!";
                return RedirectToAction("Profile", "Company");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Upload failed: {ex.Message}";
                return RedirectToAction("Profile", "Company");
            }
        }

        // GET: /User/Companies (Admin)
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Companies(int pageNumber = 1, int pageSize = 10, CancellationToken token = default)
        {
            var companies = await _userService.GetUsersAsync<Company, CompanyResponseModel>(pageNumber, pageSize, token);
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            return View(companies);
        }

        // GET: /User/Customers (Admin)
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Customers(int pageNumber = 1, int pageSize = 10, CancellationToken token = default)
        {
            var customers = await _userService.GetUsersAsync<Customer, CustomerResponseModel>(pageNumber, pageSize, token);
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            return View(customers);
        }

        // GET: /User/Admins (Admin)
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Admins(int pageNumber = 1, int pageSize = 10, CancellationToken token = default)
        {
            var admins = await _userService.GetUsersAsync<Admin, AdminResponseModel>(pageNumber, pageSize, token);
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            return View(admins);
        }

        // POST: /User/ActivateCompany (Admin)
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivateCompany(int companyId, CancellationToken token = default)
        {
            try
            {
                await _userService.ActivateCompanyAsync(companyId, token);
                TempData["Success"] = "Company activated successfully!";
                return RedirectToAction("Companies");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Companies");
            }
        }
    }
}