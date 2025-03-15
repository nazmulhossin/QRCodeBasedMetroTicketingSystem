using QRCodeBasedMetroTicketingSystem.Application.Common.Models.DataTables;
using QRCodeBasedMetroTicketingSystem.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace QRCodeBasedMetroTicketingSystem.Application.Interfaces.Repositories
{
    public interface IRepository<TEntity> where TEntity : class
    {
        //Task<IEnumerable<TEntity>> GetAllAsync();
        //Task<TEntity> GetByIdAsync(object id);
        //Task InsertAsync(TEntity entity);
        //Task UpdateAsync(TEntity entity);
        //Task DeleteAsync(object id);
        Task<DataTablesResponse<TResult>> GetDataTablesResponseAsync<TResult>(
            DataTablesRequest request,
            IQueryable<TEntity> query,
            Expression<Func<TEntity, TResult>> selector,
            Expression<Func<TEntity, bool>> globalSearchFilter = null);
    }
}
