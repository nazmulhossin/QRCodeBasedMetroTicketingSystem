using QRCodeBasedMetroTicketingSystem.Application.Common.Models.DataTables;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Repositories;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services;
using QRCodeBasedMetroTicketingSystem.Application.DTOs;
using QRCodeBasedMetroTicketingSystem.Domain.Entities;
using QRCodeBasedMetroTicketingSystem.Application.Common.Result;

namespace QRCodeBasedMetroTicketingSystem.Infrastructure.Services
{
    public class StationService : IStationService
    {
        private readonly IStationRepository _stationRepository;

        public StationService(IStationRepository stationRepository)
        {
            _stationRepository = stationRepository;
        }

        public async Task<DataTablesResponse<StationDto>> GetStationsDataTableAsync(DataTablesRequest request)
        {
            return await _stationRepository.GetStationsDataTableAsync(request);
        }

        public async Task<StationCreationDto> GetStationCreationModelAsync()
        {
            var stations = await _stationRepository.GetStationsOrderedAsync();
            return new StationCreationDto { Stations = stations };
        }

        public async Task<bool> StationExistsByNameAsync(string stationName)
        {
            return await _stationRepository.StationExistsByNameAsync(stationName);
        }

        public async Task<bool> StationExistsByNameAsync(string stationName, int? excludeStationId = null)
        {
            return await _stationRepository.StationExistsByNameAsync(stationName, excludeStationId);
        }

        public async Task<Result> CreateStationAsync(StationCreationDto model)
        {
            try
            {
                // Calculate the new station's order
                int newOrder = 1;
                if (model.InsertAfterStationId.HasValue)
                {
                    var previousStation = await _stationRepository.GetStationByIdAsync(model.InsertAfterStationId.Value);
                    newOrder = previousStation?.Order + 1 ?? 1;

                    // Update the order of subsequent stations
                    await _stationRepository.UpdateSubsequentStationOrdersAsync(newOrder, 1);
                }

                // Create the new station
                var newStation = new Station
                {
                    Name = model.StationName!,
                    Address = model.Address!,
                    Latitude = model.Latitude ?? 0.0M,
                    Longitude = model.Longitude ?? 0.0M,
                    Status = model.Status!,
                    Order = newOrder
                };

                await _stationRepository.AddStationAsync(newStation);
                await _stationRepository.SaveChangesAsync();

                // Handle distances
                if (model.Distances != null && model.Distances.Count != 0)
                {
                    if (model.Distances.Count == 2)
                    {
                        var stationIds = model.Distances.Keys.ToList();
                        int station1Id = stationIds[0];
                        int station2Id = stationIds[1];

                        // Delete existing distance record between the two stations
                        await _stationRepository.DeleteDistanceBetweenAsync(station1Id, station2Id);
                        await _stationRepository.SaveChangesAsync();
                    }

                    // Add new distances
                    foreach (var d in model.Distances)
                    {
                        int fromStation = newStation.Id;
                        int toStation = d.Key;
                        decimal distance = d.Value;

                        await _stationRepository.AddStationDistanceAsync(fromStation, toStation, distance);
                    }

                    // Save all changes at once
                    await _stationRepository.SaveChangesAsync();
                }

                return Result.Success("Station has been added successfully.");
            }
            catch (Exception ex)
            {
                return Result.Failure($"An error occurred while adding the station: {ex.Message}");
            }
        }

        public async Task<StationEditDto?> GetStationEditModelAsync(int StationId)
        {
            var station = await _stationRepository.GetStationByIdAsync(StationId);
            if (station == null)
            {
                return null;
            }

            // Get adjacent distances with adjacent station id and name.
            var adjacentDistances = await _stationRepository.GetAdjacentDistancesAsync(StationId);

            return new StationEditDto
            {
                StationId = station.Id,
                StationName = station.Name,
                Address = station.Address,
                Latitude = station.Latitude,
                Longitude = station.Longitude,
                Status = station.Status,
                Distances = adjacentDistances
            };
        }

        public async Task<Result> UpdateStationAsync(StationEditDto model)
        {
            try
            {
                var station = await _stationRepository.GetStationByIdAsync(model.StationId);
                if (station == null)
                {
                    return Result.Failure("Station not found.");
                }

                // Update station details
                station.Name = model.StationName!;
                station.Address = model.Address!;
                station.Latitude = model.Latitude ?? 0.0M;
                station.Longitude = model.Longitude ?? 0.0M;
                station.Status = model.Status!;

                // Update distances
                if (model.Distances != null && model.Distances.Count > 0)
                {
                    foreach (var d in model.Distances)
                    {
                        int fromStation = model.StationId;
                        int toStation = d.AdjacentStationId;
                        decimal newDistance = d.Distance;

                        await _stationRepository.UpdateStationDistanceAsync(fromStation, toStation, newDistance);
                    }
                }

                await _stationRepository.SaveChangesAsync();
                return Result.Success("Station details updated successfully.");
            }
            catch (Exception ex)
            {
                return Result.Failure($"An error occurred while editing the station: {ex.Message}");
            }
        }

        public async Task<StationDeletionDto?> GetStationDeletionModelAsync(int StationId)
        {
            var station = await _stationRepository.GetStationByIdAsync(StationId);
            if (station == null)
            {
                return null;
            }

            // Get adjacent distances with adjacent station id and name.
            var adjacentDistances = await _stationRepository.GetAdjacentDistancesAsync(StationId);
 
            return new StationDeletionDto
            {
                StationId = station.Id,
                StationName = station.Name,
                Address = station.Address,
                Latitude = station.Latitude,
                Longitude = station.Longitude,
                Status = station.Status,
                Distances = adjacentDistances
            };
        }

        public async Task<Result> DeleteStationAsync(int stationId)
        {
            try
            {
                var station = await _stationRepository.GetStationByIdAsync(stationId);
                if (station == null)
                {
                    return Result.Failure("Station not found.");
                }

                // Get the adjacent distances
                var adjacentDistances = await _stationRepository.GetAdjacentDistancesAsync(stationId);

                // If two adjacent distances are found, add a new distance between the remaining two stations
                if (adjacentDistances.Count == 2)
                {
                    var distance1 = adjacentDistances.FirstOrDefault();
                    var distance2 = adjacentDistances.LastOrDefault();

                    // Calculate new distance by adding the two distances together
                    decimal newDistance = distance1!.Distance + distance2!.Distance;

                    // Determine the two adjacent station's id
                    int adjacentStationId1 = distance1.AdjacentStationId;
                    int adjacentStationId2 = distance2.AdjacentStationId;

                    // Delete the existing distances involving the station
                    await _stationRepository.DeleteDistanceBetweenAsync(stationId, adjacentStationId1);
                    await _stationRepository.DeleteDistanceBetweenAsync(stationId, adjacentStationId2);

                    // Add the new distance to the database
                    await _stationRepository.AddStationDistanceAsync(adjacentStationId1, adjacentStationId2, newDistance);
                }
                else if (adjacentDistances.Count == 1)
                {
                    int adjacentStationId = adjacentDistances.FirstOrDefault().AdjacentStationId;
                    await _stationRepository.DeleteDistanceBetweenAsync(stationId, adjacentStationId);
                }

                _stationRepository.DeleteStationAsync(station); // Remove the station itself
                await _stationRepository.SaveChangesAsync();

                // Update the order of subsequent stations
                await _stationRepository.UpdateSubsequentStationOrdersAsync(station.Order + 1, -1);

                return Result.Success("Station has been successfully deleted.");
            }
            catch (Exception ex)
            {
                return Result.Failure($"Failed to delete the station: {ex.Message}");
            }
        }
    }
}
