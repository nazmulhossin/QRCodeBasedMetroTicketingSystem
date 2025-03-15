using Microsoft.EntityFrameworkCore;
using QRCodeBasedMetroTicketingSystem.Application.Common.Models.DataTables;
using QRCodeBasedMetroTicketingSystem.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Repositories;
using System.Linq.Expressions;
using QRCodeBasedMetroTicketingSystem.Application.DTOs;

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

        //public async Task<IEnumerable<TEntity>> GetAllAsync()
        //{
        //    return await _dbSet.ToListAsync();
        //}

        //public async Task<TEntity> GetByIdAsync(object id)
        //{
        //    return await _dbSet.FindAsync(id);
        //}

        //public async Task InsertAsync(TEntity entity)
        //{
        //    await _dbSet.AddAsync(entity);
        //    await _context.SaveChangesAsync();
        //}

        //public async Task UpdateAsync(TEntity entity)
        //{
        //    _dbSet.Update(entity);
        //    await _context.SaveChangesAsync();
        //}

        //public async Task DeleteAsync(object id)
        //{
        //    var entity = await GetByIdAsync(id);
        //    _dbSet.Remove(entity);
        //    await _context.SaveChangesAsync();
        //}

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
