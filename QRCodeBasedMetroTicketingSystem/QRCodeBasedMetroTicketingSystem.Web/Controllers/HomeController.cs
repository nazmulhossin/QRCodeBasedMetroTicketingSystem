using Microsoft.AspNetCore.Mvc;

namespace QRCodeBasedMetroTicketingSystem.Web.Controllers;

[Route("")]
public class HomeController : Controller
{
    public IActionResult Root()
    {
        return RedirectToAction("Index", "Home");
    }

    [Route("Home")]
    public IActionResult Index()
    {
        return View();
    }

    [Route("MapAndRoutes")]
    public IActionResult MapAndRoutes()
    {
        return View();
    }
}
