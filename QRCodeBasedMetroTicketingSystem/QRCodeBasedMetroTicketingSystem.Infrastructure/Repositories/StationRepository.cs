using AutoMapper;
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
        private readonly IMapper _mapper;

        public StationRepository(ApplicationDbContext context, IMapper mapper) : base(context)
        {
            _mapper = mapper;
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
                Id = s.Id,
                Name = s.Name,
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

        public async Task<List<StationListDto>> GetAllStationsOrderedAsync()
        {
            var stationList = await _dbSet.OrderBy(s => s.Order).ToListAsync();
            return _mapper.Map<List<StationListDto>>(stationList);
        }

        public async Task<Station?> GetStationByIdAsync(int stationId)
        {
            return await _dbSet.FindAsync(stationId);
        }

        public async Task<bool> StationExistsByNameAsync(string stationName, int? excludeStationId = null)
        {
            return await _dbSet.AnyAsync(s => s.Name == stationName && (!excludeStationId.HasValue || s.Id != excludeStationId.Value));
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

        public async Task DeleteStationAsync(Station station)
        {
            _dbSet.Remove(station);
        }
    }
}
