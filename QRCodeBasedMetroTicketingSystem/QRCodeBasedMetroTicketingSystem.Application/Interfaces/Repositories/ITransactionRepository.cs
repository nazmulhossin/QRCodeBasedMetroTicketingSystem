using QRCodeBasedMetroTicketingSystem.Domain.Entities;

namespace QRCodeBasedMetroTicketingSystem.Application.Interfaces.Repositories
{
    public interface ITransactionRepository : IRepository<Transaction>
    {
        Task CreateAsync(Transaction transaction);
        Task<Transaction?> GetByIdAsync(int id);
        Task<Transaction?> GetByReferenceAsync(string transactionReference);
        Task<IEnumerable<Transaction>> GetByWalletIdAsync(int walletId, int limit = 10);
    }
}
