using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QRCodeBasedMetroTicketingSystem.Application.Common.Models.DataTables;
using QRCodeBasedMetroTicketingSystem.Application.DTOs;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services;
using QRCodeBasedMetroTicketingSystem.Domain.Entities;
using QRCodeBasedMetroTicketingSystem.Infrastructure.Data;
using QRCodeBasedMetroTicketingSystem.Web.Areas.Admin.ViewModels;
using System.Linq;

namespace QRCodeBasedMetroTicketingSystem.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class StationController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IStationService _stationService;
        private readonly IMapper _mapper;

        public StationController(ApplicationDbContext db, IStationService stationService, IMapper mapper)
        {
            _db = db;
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
                viewModel.Stations = _mapper.Map<List<StationListViewModel>>(stationCreationModel.Stations);
                return View(viewModel);
            }

            // Check if a station with the same name already exists
            bool exists = await _stationService.StationExistsByNameAsync(viewModel.StationName);
            if (exists)
            {
                var stationCreationModel = await _stationService.GetStationCreationModelAsync();
                viewModel.Stations = _mapper.Map<List<StationListViewModel>>(stationCreationModel.Stations);
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
            var station = await _db.Stations.FindAsync(id);
            if (station == null)
            {
                return NotFound();
            }

            // Load adjacent distances
            var adjacentDistances = await _db.StationDistances
                .Where(d => d.Station1Id == id || d.Station2Id == id)
                .Select(d => new AdjacentStationDistanceViewModel
                {
                    StationId = id,
                    AdjacentStationId = d.Station1Id == id ? d.Station2Id : d.Station1Id,
                    StationName = _db.Stations.FirstOrDefault(s => s.StationId == (d.Station1Id == id ? d.Station2Id : d.Station1Id))!.StationName!,
                    Distance = d.Distance
                })
                .ToListAsync();

            var model = new StationDeleteViewModel
            {
                StationId = station.StationId,
                StationName = station.StationName,
                Address = station.Address,
                Latitude = station.Latitude,
                Longitude = station.Longitude,
                Status = station.Status,
                Distances = adjacentDistances
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(StationDeleteViewModel model)
        {
            try
            {
                var station = await _db.Stations.FindAsync(model.StationId);
                if (station == null)
                {
                    TempData["ErrorMessage"] = "Station not found.";
                    return RedirectToAction("Index");
                }

                // Get the adjacent distances
                var adjacentDistances = await _db.StationDistances
                    .Where(d => d.Station1Id == model.StationId || d.Station2Id == model.StationId)
                    .ToListAsync();

                // If two adjacent distances are found, add a new distance between the remaining two stations
                if (adjacentDistances.Count == 2)
                {
                    var distance1 = adjacentDistances.FirstOrDefault(d => d.Station1Id == model.StationId || d.Station2Id == model.StationId);
                    var distance2 = adjacentDistances.LastOrDefault(d => d.Station1Id == model.StationId || d.Station2Id == model.StationId);

                    // Calculate new distance by adding the two distances together
                    decimal newDistance = distance1!.Distance + distance2!.Distance;

                    // Determine the two adjacent stations that are left
                    int adjacentStationId1 = distance1.Station1Id == model.StationId ? distance1.Station2Id : distance1.Station1Id;
                    int adjacentStationId2 = distance2.Station1Id == model.StationId ? distance2.Station2Id : distance2.Station1Id;

                    // Create a new StationDistance record for the adjacent stations
                    var newStationDistance = new StationDistance
                    {
                        Station1Id = adjacentStationId1,
                        Station2Id = adjacentStationId2,
                        Distance = newDistance
                    };

                    // Add the new distance to the database
                    _db.StationDistances.Add(newStationDistance);
                }

                // Remove the existing distances involving the station
                _db.StationDistances.RemoveRange(adjacentDistances);

                // Remove the station itself
                _db.Stations.Remove(station);
                await _db.SaveChangesAsync();

                // Update the order of subsequent stations
                await _db.Stations
                    .Where(s => s.Order > station.Order)
                    .ExecuteUpdateAsync(setters => setters.SetProperty(s => s.Order, s => s.Order - 1));

                TempData["SuccessMessage"] = "Station has been successfully deleted.";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Failed to delete the station.";
            }

            return RedirectToAction("Index");
        }
    }
}
