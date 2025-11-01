using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace QRCodeBasedMetroTicketingSystem.Web.Areas.User.Controllers
{
    [Area("User")]
    [Authorize(Roles = "User")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return RedirectToAction("Dashboard", "MyAccount", new { area = "User" });
        }
    }
}
