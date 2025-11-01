using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QRCodeBasedMetroTicketingSystem.Application.Common.Models.DataTables;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services;
using QRCodeBasedMetroTicketingSystem.Web.Areas.Admin.ViewModels;
using QRCodeBasedMetroTicketingSystem.Web.Models;

namespace QRCodeBasedMetroTicketingSystem.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(AuthenticationSchemes = AuthSchemes.AdminScheme, Roles = ApplicationRoles.Admin)]
    public class FareAndDistanceController : Controller
    {
        private readonly IFareAndDistanceService _fareAndDistanceService;
        private readonly IMapper _mapper;

        public FareAndDistanceController(IFareAndDistanceService fareAndDistanceService, IMapper mapper)
        {
            _fareAndDistanceService = fareAndDistanceService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var distanceAndFareDto = await _fareAndDistanceService.GetFareAndDistanceModel();
            var viewModel = _mapper.Map<FareAndDistancesViewModel>(distanceAndFareDto);
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> GetFareDistanceData(DataTablesRequest request, int? fromStationId, int? toStationId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _fareAndDistanceService.GetFareDistanceDataTablesAsync(request, fromStationId, toStationId);
            return Json(response);
        }
    }
}
