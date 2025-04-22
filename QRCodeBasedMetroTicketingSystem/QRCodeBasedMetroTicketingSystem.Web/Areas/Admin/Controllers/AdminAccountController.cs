using Microsoft.AspNetCore.Mvc;

namespace QRCodeBasedMetroTicketingSystem.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminAccountController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }
    }
}
