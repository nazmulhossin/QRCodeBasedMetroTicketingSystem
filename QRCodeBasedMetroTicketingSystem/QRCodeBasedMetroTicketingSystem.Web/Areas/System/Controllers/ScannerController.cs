using Microsoft.AspNetCore.Mvc;
using QRCodeBasedMetroTicketingSystem.Application.DTOs;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services;
using QRCodeBasedMetroTicketingSystem.Domain.Entities;
using QRCodeBasedMetroTicketingSystem.Web.Areas.System.ViewModels;

namespace QRCodeBasedMetroTicketingSystem.Web.Areas.System.Controllers
{
    [Area("System")]
    public class ScannerController : Controller
    {
        private readonly IStationService _stationService;
        private readonly ITicketScanService _ticketScanService;

        public ScannerController(IStationService stationService, ITicketScanService ticketScanService)
        {
            _stationService = stationService;
            _ticketScanService = ticketScanService;
        }

        public async Task<IActionResult> Index()
        {
            var stationList = await _stationService.GetAllStationsOrderedAsync();
            var viewModel = new SelectStationViewModel
            {
                StationList = stationList
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Index(SelectStationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var stationList = await _stationService.GetAllStationsOrderedAsync();
                model.StationList = stationList;
                return View(model);
            }

            var stationName = await _stationService.GetStationNameByIdAsync(model.StationId.Value);
            if (stationName == null)
            {
                ModelState.AddModelError("StationId", "Please select a valid station");
                var stationList = await _stationService.GetAllStationsOrderedAsync();
                model.StationList = stationList;
                return View(model);
            }

            return RedirectToAction("ScanTicket", new { model.StationId });
        }

        public async Task<IActionResult> ScanTicket(int stationId)
        {
            var stationName = await _stationService.GetStationNameByIdAsync(stationId);
            if (stationName == null)
            {
                return RedirectToAction("Index");
            }

            var viewModel = new SelectStationViewModel
            {
                StationId = stationId,
                StationName = stationName
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ScanTicket([FromBody] ScanTicketRequest request)
        {
            if (!ModelState.IsValid)
            {
                return Json(new ScanTicketResponseDto { IsValid = false, Message = "Invalid" });
            }

            var result = await _ticketScanService.ProcessTicketScanAsync(request);
            return Json(result);
        }
    }
}
