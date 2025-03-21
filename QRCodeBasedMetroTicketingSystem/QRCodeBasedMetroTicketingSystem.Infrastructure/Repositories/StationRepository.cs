using Microsoft.EntityFrameworkCore;
using QRCodeBasedMetroTicketingSystem.Application.Common.Models.DataTables;
using QRCodeBasedMetroTicketingSystem.Application.DTOs;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Repositories;
using QRCodeBasedMetroTicketingSystem.Domain.Entities;
using QRCodeBasedMetroTicketingSystem.Infrastructure.Data;
using System.Linq.Expressions;


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
                searchFilter = s => s.Name.ToLower().Contains(searchValue) ||
                                    s.Address.ToLower().Contains(searchValue);
            }

            // Define the selector to map entity to DTO
            Expression<Func<Station, StationDto>> selector = s => new StationDto
            {
                StationId = s.Id,
                StationName = s.Name,
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
                            ? query.OrderBy(s => s.Name)
                            : query.OrderByDescending(s => s.Name);
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
                    StationId = s.Id,
                    StationName = s.Name,
                    Order = s.Order
                })
                .ToListAsync();
        }

        public async Task<Station?> GetStationByIdAsync(int stationId)
        {
            return await _dbSet.FindAsync(stationId);
        }

        public async Task<bool> StationExistsByNameAsync(string stationName, int? excludeStationId = null)
        {
            return await _dbSet
                .AnyAsync(s => s.Name == stationName && (!excludeStationId.HasValue || s.Id != excludeStationId.Value));
        }

        public async Task UpdateSubsequentStationOrdersAsync(int fromOrder, int step)
        {
            await _dbSet
                .Where(s => s.Order >= fromOrder)
                .ExecuteUpdateAsync(setters => setters.SetProperty(s => s.Order, s => s.Order + step));
        }

        public async Task AddStationAsync(Station station)
        {
            await _dbSet.AddAsync(station);
        }

        public void DeleteStationAsync(Station station)
        {
            _dbSet.Remove(station);
        }

        public async Task DeleteDistanceBetweenAsync(int station1Id, int station2Id)
        {
            var distance = await _db.StationDistances
                .Where(d => (d.Station1Id == station1Id && d.Station2Id == station2Id) ||
                            (d.Station1Id == station2Id && d.Station2Id == station1Id))
                .ToListAsync();

            _db.StationDistances.RemoveRange(distance);
        }

        public async Task AddStationDistanceAsync(int fromStation, int toStation, decimal distance)
        {
            var stationDistance = new StationDistance
            {
                Station1Id = fromStation,
                Station2Id = toStation,
                Distance = distance
            };

            await _db.StationDistances.AddAsync(stationDistance);
        }

        public async Task UpdateStationDistanceAsync(int fromStation, int toStation, decimal newDistance)
        {
            await _db.StationDistances
                .Where(s =>
                    (s.Station1Id == fromStation && s.Station2Id == toStation) ||
                    (s.Station1Id == toStation && s.Station2Id == fromStation))
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(s => s.Distance, s => newDistance));
        }

        public async Task<List<AdjacentStationDistanceDto>> GetAdjacentDistancesAsync(int stationId)
        {
            return await _db.StationDistances
                .Where(d => d.Station1Id == stationId || d.Station2Id == stationId)
                .Select(d => new AdjacentStationDistanceDto
                {
                    StationId = stationId,
                    AdjacentStationId = d.Station1Id == stationId ? d.Station2Id : d.Station1Id,
                    StationName = _dbSet.FirstOrDefault(s => s.Id == (d.Station1Id == stationId ? d.Station2Id : d.Station1Id))!.Name!,
                    Distance = d.Distance
                })
                .ToListAsync();
        }

        public async Task<List<Station>> GetAllStationsOrderedAsync()
        {
            return await _db.Stations.OrderBy(s => s.Order).ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
