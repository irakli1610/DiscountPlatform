using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using offers.itacademy.ge.Application.Interfaces.ServiceInterfaces;
using offers.itacademy.ge.Application.Models.Users.Admin;
using System.Threading;

namespace offers.itacademy.ge.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IUserService _userService;

        public AdminController(IUserService userService)
        {
            _userService = userService;
        }

        public IActionResult Profile()
        {
            return View();
        }

        // GET: /Admin/RegisterAdmin
        [HttpGet]
        public IActionResult RegisterAdmin()
        {
            return View(new AdminRequestModel());
        }

        // POST: /Admin/RegisterAdmin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterAdmin(AdminRequestModel model, CancellationToken token = default)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                await _userService.RegisterAsync<AdminRequestModel, AdminResponseModel>(model, token);
                TempData["Success"] = "Admin registered successfully!";
                return RedirectToAction("Profile");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }
    }
}