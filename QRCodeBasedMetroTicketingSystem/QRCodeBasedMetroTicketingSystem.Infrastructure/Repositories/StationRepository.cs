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
        public StationRepository(ApplicationDbContext context) : base(context)
        {
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
                query = ApplyOrdering(query, orderColumn, orderDir);
            }
            else
            {
                // Default ordering
                query = query.OrderBy(s => s.Order);
            }

            // Use the base method to handle pagination and create response
            return await GetDataTablesResponseAsync(request, query, selector, searchFilter);
        }

        private IQueryable<Station> ApplyOrdering(IQueryable<Station> query, string orderColumn, string orderDir)
        {
            switch (orderColumn)
            {
                case "Order":
                    return orderDir == "asc"
                        ? query.OrderBy(s => s.Order)
                        : query.OrderByDescending(s => s.Order);
                case "StationName":
                    return orderDir == "asc"
                        ? query.OrderBy(s => s.StationName)
                        : query.OrderByDescending(s => s.StationName);
                case "Status":
                    return orderDir == "asc"
                        ? query.OrderBy(s => s.Status)
                        : query.OrderByDescending(s => s.Status);
                default:
                    return query.OrderBy(s => s.Order);
            }
        }
    }
}
