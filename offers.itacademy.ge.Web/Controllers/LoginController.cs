using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using offers.itacademy.ge.Application.Interfaces.ServiceInterfaces;
using offers.itacademy.ge.Application.Models.Users;
using offers.itacademy.ge.Application.Models.Users.Admin;
using offers.itacademy.ge.Application.Models.Users.Company;
using offers.itacademy.ge.Application.Models.Users.Customer;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace offers.itacademy.ge.Web.Controllers
{
    public class LoginController : Controller
    {
        private readonly IUserService _userService;

        public LoginController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            return View(new UserLoginRequestModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserLoginRequestModel request, CancellationToken token, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(request);
            }

            string jwtTokenString;
            try
            {
                jwtTokenString = await _userService.AuthenticateAsync(request, token);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password. Please try again.");
                return View(request);
            }

            if (string.IsNullOrEmpty(jwtTokenString))
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
                return View(request);
            }

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(jwtTokenString);

            var username = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name || c.Type == "unique_name")?.Value;
            var role = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role || c.Type == "role")?.Value;
            var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(role))
            {
                ModelState.AddModelError(string.Empty, "Invalid token data.");
                return View(request);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role),
                new Claim("UserId", userId)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = jwtToken.ValidTo
                });

            Response.Cookies.Append("JwtToken", jwtTokenString, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddMinutes(10000)
            });

            Response.Cookies.Append("UserId", userId, new CookieOptions { HttpOnly = true, Secure = true, SameSite = SameSiteMode.Lax });
            Response.Cookies.Append("UserName", username, new CookieOptions { HttpOnly = true, Secure = true, SameSite = SameSiteMode.Lax });
            Response.Cookies.Append("UserRole", role, new CookieOptions { HttpOnly = true, Secure = true, SameSite = SameSiteMode.Lax });

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return role switch
            {
                "Customer" => RedirectToAction("Profile", "Customer"),
                "Company" => RedirectToAction("Profile", "Company"),
                "Admin" => RedirectToAction("Profile", "Admin"),
                _ => RedirectToAction("Index", "Home")
            };
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            Response.Cookies.Delete("UserName");
            Response.Cookies.Delete("UserRole");
            Response.Cookies.Delete("UserId");
            Response.Cookies.Delete("JwtToken");

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            ViewBag.IsAdmin = User.IsInRole("Admin");
            return View();
        }

        [HttpPost]
        public IActionResult Register(string role)
        {
            return role switch
            {
                "Customer" => RedirectToAction("RegisterCustomer"),
                "Company" => RedirectToAction("RegisterCompany"),
                "Admin" when User.IsInRole("Admin") => RedirectToAction("RegisterAdmin", "Admin"),
                _ => RedirectToAction("Register")
            };
        }

        [HttpGet]
        public IActionResult RegisterCustomer()
        {
            return View(new CustomerRequestModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterCustomer(CustomerRequestModel model, CancellationToken token = default)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                await _userService.RegisterAsync<CustomerRequestModel, CustomerResponseModel>(model, token);
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult RegisterCompany()
        {
            return View(new CompanyRequestModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterCompany(CompanyRequestModel model, CancellationToken token = default)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                await _userService.RegisterAsync<CompanyRequestModel, CompanyResponseModel>(model, token);
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }
    }
}