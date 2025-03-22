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
        private readonly ICacheService _cacheService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly string CacheKey = "Distance";

        public StationService(IUnitOfWork unitOfWork, ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
        }

        public async Task<DataTablesResponse<StationDto>> GetStationsDataTableAsync(DataTablesRequest request)
        {
            return await _unitOfWork.StationRepository.GetStationsDataTableAsync(request);
        }

        public async Task<StationCreationDto> GetStationCreationModelAsync()
        {
            var stations = await _unitOfWork.StationRepository.GetAllStationsOrderedAsync();
            return new StationCreationDto { Stations = stations };
        }

        public async Task<bool> StationExistsByNameAsync(string stationName)
        {
            return await _unitOfWork.StationRepository.StationExistsByNameAsync(stationName);
        }

        public async Task<bool> StationExistsByNameAsync(string stationName, int? excludeStationId = null)
        {
            return await _unitOfWork.StationRepository.StationExistsByNameAsync(stationName, excludeStationId);
        }

        public async Task<Result> CreateStationAsync(StationCreationDto model)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Calculate the new station's order
                int newOrder = 1;
                if (model.InsertAfterStationId.HasValue)
                {
                    var previousStation = await _unitOfWork.StationRepository.GetStationByIdAsync(model.InsertAfterStationId.Value);
                    newOrder = previousStation?.Order + 1 ?? 1;

                    // Update the order of subsequent stations
                    await _unitOfWork.StationRepository.UpdateSubsequentStationOrdersAsync(newOrder, 1);
                }

                // Create the new station
                var newStation = new Station
                {
                    Name = model.Name!,
                    Address = model.Address!,
                    Latitude = model.Latitude ?? 0.0M,
                    Longitude = model.Longitude ?? 0.0M,
                    Status = model.Status!,
                    Order = newOrder
                };

                await _unitOfWork.StationRepository.AddStationAsync(newStation);

                // Handle distances
                if (model.Distances != null && model.Distances.Count != 0)
                {
                    if (model.Distances.Count == 2)
                    {
                        var stationIds = model.Distances.Keys.ToList();
                        int station1Id = stationIds[0];
                        int station2Id = stationIds[1];

                        // Delete existing distance record between the two stations
                        await _unitOfWork.StationDistanceRepository.DeleteDistanceBetweenAsync(station1Id, station2Id);
                    }

                    // Add new distances
                    foreach (var d in model.Distances)
                    {
                        int fromStation = newStation.Id;
                        int toStation = d.Key;
                        decimal distance = d.Value;

                        await _unitOfWork.StationDistanceRepository.AddStationDistanceAsync(fromStation, toStation, distance);
                    }
                }

                await _unitOfWork.SaveChangesAsync();         // Commit all changes at once
                await _unitOfWork.CommitTransactionAsync();   // Commit the transaction
                await _cacheService.RemoveAsync(CacheKey);    // Clean Distance from Cache

                return Result.Success("Station has been added successfully.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(); // Rollback the transaction if any error occurs
                return Result.Failure($"An error occurred while adding the station: {ex.Message}");
            }
        }

        public async Task<StationEditDto?> GetStationEditModelAsync(int StationId)
        {
            var station = await _unitOfWork.StationRepository.GetStationByIdAsync(StationId);
            if (station == null)
            {
                return null;
            }

            // Get adjacent distances with adjacent station id and name.
            var adjacentDistances = await _unitOfWork.StationDistanceRepository.GetAdjacentDistancesAsync(StationId);

            return new StationEditDto
            {
                Id = station.Id,
                Name = station.Name,
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
                await _unitOfWork.BeginTransactionAsync();

                var station = await _unitOfWork.StationRepository.GetStationByIdAsync(model.Id);
                if (station == null)
                {
                    return Result.Failure("Station not found.");
                }

                // Update station details
                station.Name = model.Name!;
                station.Address = model.Address!;
                station.Latitude = model.Latitude ?? 0.0M;
                station.Longitude = model.Longitude ?? 0.0M;
                station.Status = model.Status!;

                // Update distances
                if (model.Distances != null && model.Distances.Count > 0)
                {
                    foreach (var d in model.Distances)
                    {
                        int fromStation = model.Id;
                        int toStation = d.AdjacentStationId;
                        decimal newDistance = d.Distance;

                        await _unitOfWork.StationDistanceRepository.UpdateStationDistanceAsync(fromStation, toStation, newDistance);
                    }
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
                await _cacheService.RemoveAsync("Distance");

                return Result.Success("Station details updated successfully.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return Result.Failure($"An error occurred while editing the station: {ex.Message}");
            }
        }

        public async Task<StationDeletionDto?> GetStationDeletionModelAsync(int StationId)
        {
            var station = await _unitOfWork.StationRepository.GetStationByIdAsync(StationId);
            if (station == null)
            {
                return null;
            }

            // Get adjacent distances with adjacent station id and name.
            var adjacentDistances = await _unitOfWork.StationDistanceRepository.GetAdjacentDistancesAsync(StationId);

            return new StationDeletionDto
            {
                Id = station.Id,
                Name = station.Name,
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
                await _unitOfWork.BeginTransactionAsync();

                var station = await _unitOfWork.StationRepository.GetStationByIdAsync(stationId);
                if (station == null)
                {
                    return Result.Failure("Station not found.");
                }

                // Get the adjacent distances
                var adjacentDistances = await _unitOfWork.StationDistanceRepository.GetAdjacentDistancesAsync(stationId);

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
                    await _unitOfWork.StationDistanceRepository.DeleteDistanceBetweenAsync(stationId, adjacentStationId1);
                    await _unitOfWork.StationDistanceRepository.DeleteDistanceBetweenAsync(stationId, adjacentStationId2);

                    // Add the new distance to the database
                    await _unitOfWork.StationDistanceRepository.AddStationDistanceAsync(adjacentStationId1, adjacentStationId2, newDistance);
                }
                else if (adjacentDistances.Count == 1)
                {
                    int adjacentStationId = adjacentDistances.FirstOrDefault()!.AdjacentStationId;
                    await _unitOfWork.StationDistanceRepository.DeleteDistanceBetweenAsync(stationId, adjacentStationId);
                }
                
                await _unitOfWork.StationRepository.DeleteStationAsync(station); // Remove the station itself
                await _unitOfWork.StationRepository.UpdateSubsequentStationOrdersAsync(station.Order + 1, -1); // Update the order of subsequent stations
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
                await _cacheService.RemoveAsync("Distance");

                return Result.Success("Station has been successfully deleted.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return Result.Failure($"Failed to delete the station: {ex.Message}");
            }
        }
    }
}
