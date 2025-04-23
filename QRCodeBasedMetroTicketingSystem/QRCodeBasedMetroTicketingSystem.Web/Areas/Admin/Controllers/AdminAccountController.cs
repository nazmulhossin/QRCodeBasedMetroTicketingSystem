using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using QRCodeBasedMetroTicketingSystem.Web.Areas.Admin.ViewModels;
using System.Security.Claims;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services;
using QRCodeBasedMetroTicketingSystem.Web.Models;
using Microsoft.AspNetCore.Authorization;

namespace QRCodeBasedMetroTicketingSystem.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminAccountController : Controller
    {
        private readonly IAdminService _adminService;

        public AdminAccountController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(AdminLoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _adminService.ValidateAdminCredentialsAsync(model.Email, model.Password);
            if (result.Success == false)
            {
                ModelState.AddModelError("Password", "Invalid email or password.");
                return View(model);
            }

            // Create claims for the admin
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, result.AdminId.ToString()),
                new Claim(ClaimTypes.Email, model.Email),
                new Claim(ClaimTypes.Role, ApplicationRoles.Admin),
                new Claim(CustomClaimTypes.JwtToken, result.Token)
            };

            // Sign in using cookie authentication
            var identity = new ClaimsIdentity(claims, AuthSchemes.AdminScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(
                AuthSchemes.AdminScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe,
                    ExpiresUtc = DateTime.UtcNow.AddDays(1)
                });

            return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
        }

        [Authorize(AuthenticationSchemes = AuthSchemes.AdminScheme, Roles = ApplicationRoles.Admin)]
        public async Task<IActionResult> Logout()
        {
            // Sign out from the admin scheme
            await HttpContext.SignOutAsync(AuthSchemes.AdminScheme);

            return RedirectToAction("Login", "AdminAccount", new { area = "Admin" });
        }
    }
}
