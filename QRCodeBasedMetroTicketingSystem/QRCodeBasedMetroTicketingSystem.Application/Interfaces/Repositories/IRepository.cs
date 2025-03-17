using QRCodeBasedMetroTicketingSystem.Application.Common.Models.DataTables;
using System.Linq.Expressions;

namespace QRCodeBasedMetroTicketingSystem.Application.Interfaces.Repositories
{
    public interface IRepository<TEntity> where TEntity : class
    {
        Task<DataTablesResponse<TResult>> GetDataTablesResponseAsync<TResult>(
            DataTablesRequest request,
            IQueryable<TEntity> query,
            Expression<Func<TEntity, TResult>> selector,
            Expression<Func<TEntity, bool>> globalSearchFilter = null);
    }
}
