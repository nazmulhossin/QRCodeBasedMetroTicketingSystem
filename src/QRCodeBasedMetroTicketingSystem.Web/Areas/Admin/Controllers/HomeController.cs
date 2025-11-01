using Microsoft.AspNetCore.Mvc;

namespace QRCodeBasedMetroTicketingSystem.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return RedirectToAction("Login", "AdminAccount", new { area = "Admin" });
        }
    }
}
