using Microsoft.EntityFrameworkCore;
using QRCodeBasedMetroTicketingSystem.Application.Common.Models.DataTables;
using QRCodeBasedMetroTicketingSystem.Application.DTOs;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Repositories;
using QRCodeBasedMetroTicketingSystem.Domain.Entities;
using QRCodeBasedMetroTicketingSystem.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace QRCodeBasedMetroTicketingSystem.Infrastructure.Repositories
{
    public class StationRepository : Repository<Station>, IStationRepository
    {
        private readonly ApplicationDbContext _db;

        public StationRepository(ApplicationDbContext context) : base(context)
        {
            _db = context;
        }

        public async Task<DataTablesResponse<StationDto>> GetStationsDataTableAsync(DataTablesRequest request)
        {
            // Start with the base query
            var query = _dbSet.AsQueryable();

            // Define the search filter
            Expression<Func<Station, bool>>? searchFilter = null;
            if (!string.IsNullOrEmpty(request.Search?.Value))
            {
                var searchValue = request.Search.Value.ToLower();
                searchFilter = s => s.StationName.ToLower().Contains(searchValue) ||
                                    s.Address.ToLower().Contains(searchValue);
            }

            // Define the selector to map entity to DTO
            Expression<Func<Station, StationDto>> selector = s => new StationDto
            {
                StationId = s.StationId,
                StationName = s.StationName,
                Address = s.Address,
                Latitude = s.Latitude,
                Longitude = s.Longitude,
                Order = s.Order,
                Status = s.Status
            };

            // Check ordering and apply it
            if (request.Order != null && request.Order.Any())
            {
                var orderColumn = request.Columns[request.Order[0].Column].Name;
                var orderDir = request.Order[0].Dir;

                // Apply specific ordering based on column
                switch (orderColumn)
                {
                    case "Order":
                        query = orderDir == "asc"
                            ? query.OrderBy(s => s.Order)
                            : query.OrderByDescending(s => s.Order);
                        break;

                    case "StationName":
                        query = orderDir == "asc"
                            ? query.OrderBy(s => s.StationName)
                            : query.OrderByDescending(s => s.StationName);
                        break;

                    case "Status":
                        query = orderDir == "asc"
                            ? query.OrderBy(s => s.Status)
                            : query.OrderByDescending(s => s.Status);
                        break;

                    default:
                        query = query.OrderBy(s => s.Order);
                        break;
                }
            }
            else
            {
                // Default ordering
                query = query.OrderBy(s => s.Order);
            }

            // Use the base method to handle pagination and create response
            return await GetDataTablesResponseAsync(request, query, selector, searchFilter);
        }

        public async Task<List<StationListDto>> GetStationsOrderedAsync()
        {
            return await _dbSet
                .OrderBy(s => s.Order)
                .Select(s => new StationListDto
                {
                    StationId = s.StationId,
                    StationName = s.StationName,
                    Order = s.Order
                })
                .ToListAsync();
        }

        public async Task<Station> GetStationByIdAsync(int stationId)
        {
            return await _dbSet.FirstOrDefaultAsync(s => s.StationId == stationId);
        }

        public async Task<bool> StationExistsByNameAsync(string stationName)
        {
            return await _dbSet.AnyAsync(s => s.StationName == stationName);
        }

        public async Task UpdateSubsequentStationOrdersAsync(int fromOrder)
        {
            await _dbSet
                .Where(s => s.Order >= fromOrder)
                .ExecuteUpdateAsync(setters => setters.SetProperty(s => s.Order, s => s.Order + 1));
        }

        public async Task AddStationAsync(Station station)
        {
            await _dbSet.AddAsync(station);
        }

        public async Task DeleteExistingDistancesAsync(int station1Id, int station2Id)
        {
            var existingDistances = await _db.StationDistances
                .Where(d => (d.Station1Id == station1Id && d.Station2Id == station2Id) ||
                            (d.Station1Id == station2Id && d.Station2Id == station1Id))
                .ToListAsync();

            _db.StationDistances.RemoveRange(existingDistances);
        }

        public async Task AddStationDistancesAsync(int stationId, Dictionary<int, decimal> distances)
        {
            var stationDistances = distances.Select(distance => new StationDistance
            {
                Station1Id = distance.Key,
                Station2Id = stationId,
                Distance = distance.Value
            }).ToList();

            await _context.StationDistances.AddRangeAsync(stationDistances);
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
