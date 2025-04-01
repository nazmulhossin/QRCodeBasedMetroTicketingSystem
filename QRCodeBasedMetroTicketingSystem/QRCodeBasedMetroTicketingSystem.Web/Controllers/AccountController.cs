using Microsoft.AspNetCore.Mvc;

namespace QRCodeBasedMetroTicketingSystem.Web.Controllers
{
    public class AccountController : Controller
    {
        [Route("Signup")]
        public IActionResult Signup()
        {
            return View();
        }

        [Route("Login")]
        public IActionResult Login()
        {
            return View();
        }
    }
}
