using Microsoft.AspNetCore.Mvc;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services;

namespace QRCodeBasedMetroTicketingSystem.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class FareAndDistanceController : Controller
    {
        private readonly IDistanceCalculationService _distanceCalculationService;

        public FareAndDistanceController(IDistanceCalculationService distanceCalculationService)
        {
            _distanceCalculationService = distanceCalculationService;
        }

        public async Task<IActionResult> Index()
        {
            List<string> allDistances = new List<string>();
            for (int i = 1; i < 10; i++)
            {
                for (int j = 1; j <= 10; j++)
                { 
                    decimal distance = await _distanceCalculationService.GetDistanceBetweenAsync(i, j);
                    allDistances.Add(string.Format("from: {0} to: {1} distance: {2}", i, j, distance));
                }
            }
            return Json(allDistances);
        }
    }
}

