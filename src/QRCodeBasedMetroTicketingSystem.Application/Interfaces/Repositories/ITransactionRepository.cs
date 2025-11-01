using QRCodeBasedMetroTicketingSystem.Domain.Entities;

namespace QRCodeBasedMetroTicketingSystem.Application.Interfaces.Repositories
{
    public interface ITransactionRepository : IRepository<Transaction>
    {
        Task CreateAsync(Transaction transaction);
        Task<Transaction?> GetByIdAsync(int id);
        Task<Transaction?> GetByReferenceAsync(string transactionReference);
        Task<IEnumerable<Transaction>> GetByWalletIdAsync(int walletId, int limit = 10);
        Task<decimal> GetTotalTopUpsAsync(int userId, DateTime fromDate);
        Task<List<Transaction>> GetRecentTransactionsByUserIdAsync(int userId, int count = 10);
        Task<IDictionary<string, decimal>> GetMonthlyRevenueAsync(int months);
        Task<decimal> GetTotalRevenueAsync();
    }
}
