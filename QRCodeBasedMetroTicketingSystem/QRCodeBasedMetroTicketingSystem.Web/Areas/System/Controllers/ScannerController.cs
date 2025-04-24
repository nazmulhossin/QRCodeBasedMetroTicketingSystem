using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services;
using QRCodeBasedMetroTicketingSystem.Domain.Entities;
using QRCodeBasedMetroTicketingSystem.Web.Areas.Admin.ViewModels;
using QRCodeBasedMetroTicketingSystem.Web.Areas.System.ViewModels;
using System.Threading.Tasks;

namespace QRCodeBasedMetroTicketingSystem.Web.Areas.System.Controllers
{
    [Area("System")]
    public class ScannerController : Controller
    {
        private readonly IStationService _stationService;
        private readonly IMapper _mapper;

        public ScannerController(IStationService stationService, IMapper mapper)
        {
            _stationService = stationService;
            _mapper = mapper;
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

            ViewBag.StationId = stationId;
            ViewBag.StationName = stationName;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ScanTicket([FromBody] ScanTicketRequest? request)
        {
            if (request == null)
            {
                return BadRequest(new { IsValid = false, Message = "Request is null" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (string.IsNullOrEmpty(request.Token))
            {
                return Json(new
                {
                    IsValid = false,
                    Message = "Invalid QR code data"
                });
            }

            if (char.IsLower(request.Token[0]))
            {
                return Json(new
                {
                    IsValid = true,
                    Message = "Valid QR code data"
                });
            }

            return Json(new
            {
                IsValid = false,
                Message = "Invalid QR code data"
            });
        }
    }
}
