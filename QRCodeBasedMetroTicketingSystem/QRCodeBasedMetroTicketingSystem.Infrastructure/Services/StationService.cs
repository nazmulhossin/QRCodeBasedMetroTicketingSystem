using QRCodeBasedMetroTicketingSystem.Application.Common.Models.DataTables;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Repositories;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services;
using QRCodeBasedMetroTicketingSystem.Application.DTOs;
using QRCodeBasedMetroTicketingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QRCodeBasedMetroTicketingSystem.Application.Common.Result;
using Microsoft.EntityFrameworkCore;
using static System.Collections.Specialized.BitVector32;

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
                    await _stationRepository.UpdateSubsequentStationOrdersAsync(newOrder);
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
                        await _stationRepository.DeleteExistingDistancesAsync(station1Id, station2Id);
                        await _stationRepository.SaveChangesAsync();
                    }

                    // Add new distances
                    await _stationRepository.AddStationDistancesAsync(newStation.StationId, model.Distances);
                    await _stationRepository.SaveChangesAsync();                    
                }

                return Result.Success("Station has been added successfully.");
            }
            catch (Exception ex)
            {
                return Result.Failure($"An error occurred while adding the station: {ex.Message}");
            }
        }
    }
}
