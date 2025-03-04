using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRCodeBasedMetroTicketingSystem.Infrastructure.Data;

namespace QRCodeBasedMetroTicketingSystem.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class StationController : Controller
    {
        private readonly ApplicationDbContext _db;

        public StationController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var stations = _db.Stations.OrderBy(s => s.Order).ToList();
            return View(stations);
        }
    }
}
