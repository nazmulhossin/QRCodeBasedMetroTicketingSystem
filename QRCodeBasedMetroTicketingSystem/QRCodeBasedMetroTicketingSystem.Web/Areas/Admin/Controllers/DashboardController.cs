using Microsoft.AspNetCore.Mvc;

namespace QRCodeBasedMetroTicketingSystem.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
