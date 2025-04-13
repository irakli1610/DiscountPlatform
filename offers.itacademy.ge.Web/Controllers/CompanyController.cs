using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using offers.itacademy.ge.Application.Interfaces.ServiceInterfaces;
using offers.itacademy.ge.Application.Models.Users.Company;
using System.Security.Claims;
using System.Threading;

namespace offers.itacademy.ge.Web.Controllers
{
    [Authorize(Roles = "Company")]
    public class CompanyController : Controller
    {
        private readonly IUserService _userService;

        public CompanyController(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IActionResult> Profile(CancellationToken token = default)
        {
            var companyId = int.Parse(User.FindFirstValue("UserId"));
            var user = await _userService.GetByIdAsync(companyId, token);
            if (user == null) return NotFound();

            if (user is not CompanyResponseModel company)
                return BadRequest("User is not a company."); // Safety check

            ViewBag.ImageUrl = company.ImageUrl; // Cast to access ImageUrl
            return View();
        }
    }
}