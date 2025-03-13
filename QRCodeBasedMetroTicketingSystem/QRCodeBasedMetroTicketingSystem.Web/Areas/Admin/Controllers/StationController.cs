using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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

        public StationController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        public async Task<IActionResult> GetStationData(DataTablesRequest request)
        {
            try
            {
                // Get total count for pagination info
                var totalRecords = await _db.Stations.CountAsync();

                // Create query from stations
                var stationsQuery = _db.Stations.AsQueryable();

                // Apply search if present
                if (!string.IsNullOrEmpty(request.Search?.Value))
                {
                    stationsQuery = stationsQuery.Where(s =>
                        s.StationName.Contains(request.Search.Value) ||
                        s.Address.Contains(request.Search.Value));
                }

                // Get filtered count
                var filteredCount = await stationsQuery.CountAsync();

                // Apply ordering (sort)
                if (request.Order != null && request.Order.Any())
                {
                    var orderColumn = request.Columns[request.Order[0].Column].Name;
                    var orderDir = request.Order[0].Dir;

                    switch (orderColumn)
                    {
                        case "Order":
                            stationsQuery = orderDir == "asc"
                                ? stationsQuery.OrderBy(s => s.Order)
                                : stationsQuery.OrderByDescending(s => s.Order);
                            break;
                        case "StationName":
                            stationsQuery = orderDir == "asc"
                                ? stationsQuery.OrderBy(s => s.StationName)
                                : stationsQuery.OrderByDescending(s => s.StationName);
                            break;
                        case "Address":
                            stationsQuery = orderDir == "asc"
                                ? stationsQuery.OrderBy(s => s.Address)
                                : stationsQuery.OrderByDescending(s => s.Address);
                            break;
                        case "Status":
                            stationsQuery = orderDir == "asc"
                                ? stationsQuery.OrderBy(s => s.Status)
                                : stationsQuery.OrderByDescending(s => s.Status);
                            break;
                        default:
                            stationsQuery = stationsQuery.OrderBy(s => s.Order);
                            break;
                    }
                }
                else
                {
                    // Default ordering
                    stationsQuery = stationsQuery.OrderBy(s => s.Order);
                }

                // Apply pagination
                var stations = await stationsQuery
                    .Skip(request.Start)
                    .Take(request.Length)
                    .Select(s => new StationViewModel
                    {
                        StationId = s.StationId,
                        StationName = s.StationName,
                        Address = s.Address,
                        Latitude = s.Latitude,
                        Longitude = s.Longitude,
                        Order = s.Order,
                        Status = s.Status
                    }).ToListAsync();

                // Return DataTables response format
                return Json(new
                {
                    draw = request.Draw,
                    recordsTotal = totalRecords,
                    recordsFiltered = filteredCount,
                    data = stations
                });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }


        public async Task<IActionResult> Create()
        {
            var stations = await _db.Stations
                .OrderBy(s => s.Order)
                .Select(s => new StationListViewModel
                {
                    StationId = s.StationId,
                    StationName = s.StationName,
                    Order = s.Order
                }).ToListAsync();

            var model = new StationCreationViewModel { Stations = stations };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(StationCreationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // If model state is invalid, reload the form with existing data
                model.Stations = await _db.Stations
                    .OrderBy(s => s.Order)
                    .Select(s => new StationListViewModel
                    {
                        StationId = s.StationId,
                        StationName = s.StationName,
                        Order = s.Order
                    }).ToListAsync();

                return View(model);
            }

            // Check if a station with the same name already exists
            bool exists = await _db.Stations.AnyAsync(s => s.StationName == model.StationName);
            if (exists)
            {
                model.Stations = await _db.Stations
                    .OrderBy(s => s.Order)
                    .Select(s => new StationListViewModel
                    {
                        StationId = s.StationId,
                        StationName = s.StationName,
                        Order = s.Order
                    }).ToListAsync();

                ModelState.AddModelError("StationName", "A station with the same name already exists.");
                return View(model);
            }

            try
            {
                // Calculate the new station's order
                int newOrder = 1;
                if (model.InsertAfterStationId.HasValue)
                {
                    var previousStation = _db.Stations.FirstOrDefault(s => s.StationId == model.InsertAfterStationId.Value);
                    newOrder = previousStation?.Order + 1 ?? 1;

                    // Update the order of subsequent stations
                    await _db.Stations
                        .Where(s => s.Order >= newOrder)
                        .ExecuteUpdateAsync(setters => setters.SetProperty(s => s.Order, s => s.Order + 1));
                }

                // Create the new station
                var newStation = new Station
                {
                    StationName = model.StationName!,
                    Address = model.Address!,
                    Latitude = model.Latitude ?? 0.0M,
                    Longitude = model.Longitude ?? 0.0M,
                    Status = model.Status!,
                    Order = newOrder
                };

                _db.Stations.Add(newStation);
                await _db.SaveChangesAsync();

                // Save distances
                if (model.Distances != null && model.Distances.Count != 0)
                {
                    if (model.Distances.Count == 2)
                    {
                        var stationIds = model.Distances.Keys.ToList(); // Extract the two station IDs
                        int station1Id = stationIds[0];
                        int station2Id = stationIds[1];

                        // Delete existing distance record between the two stations
                        var existingDistances = _db.StationDistances
                            .Where(d => (d.Station1Id == station1Id && d.Station2Id == station2Id) ||
                                        (d.Station1Id == station2Id && d.Station2Id == station1Id))
                            .ToList();

                        _db.StationDistances.RemoveRange(existingDistances);
                        await _db.SaveChangesAsync();
                    }

                    // Add new distances
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

                    await _db.SaveChangesAsync();
                }

                TempData["SuccessMessage"] = "Station has been added successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error occurred while adding the station.";
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var station = await _db.Stations.FindAsync(id);
            if (station == null)
            {
                return NotFound();
            }

            // Load adjacent distances
            var adjacentDistances = await _db.StationDistances
                .Where(d => d.Station1Id == id || d.Station2Id == id)
                .Select(d => new DistanceViewModel
                {
                    StationId = id,
                    AdjacentStationId = d.Station1Id == id ? d.Station2Id : d.Station1Id,
                    StationName = _db.Stations.FirstOrDefault(s => s.StationId == (d.Station1Id == id ? d.Station2Id : d.Station1Id))!.StationName!,
                    Distance = d.Distance
                })
                .ToListAsync();

            var model = new StationEditViewModel
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
        public async Task<IActionResult> Edit(StationEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Check if a station with the same name already exists (excluding the current station)
            bool exists = await _db.Stations
                .AnyAsync(s => s.StationName == model.StationName && s.StationId != model.StationId);

            if (exists)
            {
                ModelState.AddModelError("StationName", "A station with the same name already exists.");
                return View(model);
            }

            try
            {
                var station = await _db.Stations.FindAsync(model.StationId);
                if (station == null)
                {
                    return NotFound();
                }

                // Update station details
                station.StationName = model.StationName!;
                station.Address = model.Address!;
                station.Latitude = model.Latitude ?? 0.0M;
                station.Longitude = model.Longitude ?? 0.0M;
                station.Status = model.Status!;

                // Delete existing distances related to this station
                var existingDistances = _db.StationDistances
                    .Where(d => d.Station1Id == model.StationId || d.Station2Id == model.StationId);

                _db.StationDistances.RemoveRange(existingDistances);

                // Save updated distances
                if (model.Distances != null && model.Distances.Count != 0)
                {
                    foreach (var distance in model.Distances)
                    {
                        var stationDistance = new StationDistance
                        {
                            Station1Id = model.StationId,
                            Station2Id = distance.AdjacentStationId,
                            Distance = distance.Distance
                        };

                        _db.StationDistances.Add(stationDistance);
                    }
                }

                await _db.SaveChangesAsync();

                TempData["SuccessMessage"] = "Station details updated successfully.";
                return RedirectToAction("Index");
            }
            catch(Exception)
            {
                TempData["ErrorMessage"] = "An error occurred while editing the station.";
                return RedirectToAction("Index");
            }
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
                .Select(d => new DistanceViewModel
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
