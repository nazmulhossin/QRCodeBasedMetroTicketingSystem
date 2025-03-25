using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services;
using QRCodeBasedMetroTicketingSystem.Infrastructure.Services;
using QRCodeBasedMetroTicketingSystem.Web.Areas.Admin.ViewModels;

namespace QRCodeBasedMetroTicketingSystem.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class FareAndDistanceController : Controller
    {
        private readonly IFareAndDistanceService _distanceAndFareService;
        private readonly IMapper _mapper;

        public FareAndDistanceController(IFareAndDistanceService distanceAndFareService, IMapper mapper)
        {
            _distanceAndFareService = distanceAndFareService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var distanceAndFareDto = await _distanceAndFareService.GetFareAndDistanceModel();
            var viewModel = _mapper.Map<FareAndDistancesViewModel>(distanceAndFareDto);
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> GetFareDistanceData(
            int? fromStationId,
            int? toStationId,
            int draw,
        int start,
            int length)
        {
            var fareDistances = await _fareCalculationService.GetFareDistancesAsync(fromStationId, toStationId);

            // Prepare the result for DataTable
            return Json(new
            {
                draw = draw,
                recordsTotal = fareDistances.Count(),
                recordsFiltered = fareDistances.Count(),
                data = fareDistances.Skip(start).Take(length).ToList()
            });
        }
    }
}
