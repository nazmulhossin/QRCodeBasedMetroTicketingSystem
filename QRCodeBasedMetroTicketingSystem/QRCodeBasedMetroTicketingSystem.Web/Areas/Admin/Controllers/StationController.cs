using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QRCodeBasedMetroTicketingSystem.Domain.Entities;
using QRCodeBasedMetroTicketingSystem.Infrastructure.Data;
using QRCodeBasedMetroTicketingSystem.Web.Areas.Admin.ViewModels;

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

        public async Task<IActionResult> Index()
        {
            var stations = await _db.Stations.OrderBy(s => s.Order).ToListAsync();
            return View(stations);
        }

        public IActionResult Create()
        {
            var stations = _db.Stations.OrderBy(s => s.Order).ToList();
            var model = new StationCreationViewModel { Stations = stations };
            return View(model);
        }

        [HttpPost]
        public IActionResult Create(StationCreationViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Calculate the new station's order
                int newOrder;
                if (model.InsertAfterStationId.HasValue)
                {
                    var previousStation = _db.Stations.FirstOrDefault(s => s.StationId == model.InsertAfterStationId.Value);
                    newOrder = previousStation?.Order + 1 ?? 1;

                    // Update the order of subsequent stations
                    var subsequentStations = _db.Stations.Where(s => s.Order >= newOrder).ToList();
                    foreach (var station in subsequentStations)
                    {
                        station.Order += 1;
                    }
                }
                else
                {
                    newOrder = 1;

                    // Update the order of all stations
                    var allStations = _db.Stations.ToList();
                    foreach (var station in allStations)
                    {
                        station.Order += 1;
                    }
                }

                // Create the new station
                var newStation = new Station
                {
                    StationName = model.StationName,
                    Address = model.Address,
                    Latitude = model.Latitude ?? 0.0M,
                    Longitude = model.Longitude ?? 0.0M,
                    Status = model.Status,
                    Order = newOrder
                };

                _db.Stations.Add(newStation);
                _db.SaveChanges();

                // Save distances
                if (model.Distances != null && model.Distances.Any())
                {
                    foreach (var distance in model.Distances)
                    {
                        var stationDistance = new StationDistance
                        {
                            Station1Id = distance.Key,
                            Station2Id = newStation.StationId,
                            Distance = distance.Value
                        };

                        _db.StationDistances.Add(stationDistance);
                    }

                    _db.SaveChanges();
                }

                return RedirectToAction("Index");
            }

            // If model state is invalid, reload the form with existing data
            model.Stations = _db.Stations.OrderBy(s => s.Order).ToList();
            return View(model);
        }
    }
}
