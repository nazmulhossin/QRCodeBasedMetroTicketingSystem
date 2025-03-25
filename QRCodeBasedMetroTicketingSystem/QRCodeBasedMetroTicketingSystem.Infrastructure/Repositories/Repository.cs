using Microsoft.EntityFrameworkCore;
using QRCodeBasedMetroTicketingSystem.Application.Common.Models.DataTables;
using QRCodeBasedMetroTicketingSystem.Infrastructure.Data;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Repositories;
using System.Linq.Expressions;

namespace QRCodeBasedMetroTicketingSystem.Infrastructure.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public async Task<DataTablesResponse<TResult>> GetDataTablesResponseAsync<TResult>(
        DataTablesRequest request,
        IQueryable<TEntity> query,
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? globalSearchFilter = null)
        {
            try
            {
                // Get total count before applying any filters
                var totalRecords = await query.CountAsync();

                // Apply global search if provided
                if (!string.IsNullOrEmpty(request.Search?.Value) && globalSearchFilter != null)
                {
                    query = query.Where(globalSearchFilter);
                }

                // Get filtered count
                var filteredRecords = await query.CountAsync();

                // Apply pagination
                query = query.Skip(request.Start).Take(request.Length);

                // Select the final data
                var data = await query.Select(selector).ToListAsync();

                // Build the response
                return new DataTablesResponse<TResult>
                {
                    Draw = request.Draw,
                    RecordsTotal = totalRecords,
                    RecordsFiltered = filteredRecords,
                    Data = data
                };
            }
            catch (Exception ex)
            {
                return new DataTablesResponse<TResult>
                {
                    Error = ex.Message,
                    Data = new List<TResult>(),
                    RecordsTotal = 0,
                    RecordsFiltered = 0
                };
            }
        }
    }
}
