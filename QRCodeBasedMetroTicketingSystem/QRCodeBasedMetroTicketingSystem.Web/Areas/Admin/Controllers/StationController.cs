using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using QRCodeBasedMetroTicketingSystem.Application.Common.Models.DataTables;
using QRCodeBasedMetroTicketingSystem.Application.DTOs;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services;
using QRCodeBasedMetroTicketingSystem.Web.Areas.Admin.ViewModels;

namespace QRCodeBasedMetroTicketingSystem.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class StationController : Controller
    {
        private readonly IStationService _stationService;
        private readonly IMapper _mapper;

        public StationController(IStationService stationService, IMapper mapper)
        {
            _stationService = stationService;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetStationData(DataTablesRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _stationService.GetStationsDataTableAsync(request);
            return Json(response);
        }

        public async Task<IActionResult> Create()
        {
            var stationCreationDto = await _stationService.GetStationCreationModelAsync();
            var viewModel = _mapper.Map<StationCreationViewModel>(stationCreationDto);
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(StationCreationViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                // If model state is invalid, reload the form with existing data
                var stationCreationModel = await _stationService.GetStationCreationModelAsync();
                viewModel.Stations = stationCreationModel.Stations;
                return View(viewModel);
            }

            // Check if a station with the same name already exists
            bool exists = await _stationService.StationExistsByNameAsync(viewModel.StationName);
            if (exists)
            {
                var stationCreationModel = await _stationService.GetStationCreationModelAsync();
                viewModel.Stations = stationCreationModel.Stations;
                ModelState.AddModelError("StationName", "A station with the same name already exists.");
                return View(viewModel);
            }

            var stationCreationDto = _mapper.Map<StationCreationDto>(viewModel);
            var result = await _stationService.CreateStationAsync(stationCreationDto);

            if (result.IsSuccess)
                TempData["SuccessMessage"] = result.Message;
            else
                TempData["ErrorMessage"] = result.Message;

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var stationEditDto = await _stationService.GetStationEditModelAsync(id);
            if (stationEditDto == null)
            {
                return NotFound();
            }

            var viewModel = _mapper.Map<StationEditViewModel>(stationEditDto);
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(StationEditViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            // Check if a station with the same name already exists (excluding the current station)
            bool exists = await _stationService.StationExistsByNameAsync(viewModel.StationName, viewModel.StationId);
            if (exists)
            {
                ModelState.AddModelError("StationName", "A station with the same name already exists.");
                return View(viewModel);
            }

            var StationEditDto = _mapper.Map<StationEditDto>(viewModel);
            var result = await _stationService.UpdateStationAsync(StationEditDto);

            if (result.IsSuccess)
                TempData["SuccessMessage"] = result.Message;
            else
                TempData["ErrorMessage"] = result.Message;

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var stationDeletionDto = await _stationService.GetStationDeletionModelAsync(id);
            if (stationDeletionDto == null)
            {
                return NotFound();
            }

            var viewModel = _mapper.Map<StationDeletionViewModel>(stationDeletionDto);
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(StationDeletionViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _stationService.DeleteStationAsync(viewModel.StationId);

            if (result.IsSuccess)
                TempData["SuccessMessage"] = result.Message;
            else
                TempData["ErrorMessage"] = result.Message;

            return RedirectToAction("Index");
        }
    }
}
