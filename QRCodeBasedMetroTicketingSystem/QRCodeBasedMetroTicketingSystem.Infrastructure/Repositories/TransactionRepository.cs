using Microsoft.EntityFrameworkCore;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Repositories;
using QRCodeBasedMetroTicketingSystem.Domain.Entities;
using QRCodeBasedMetroTicketingSystem.Infrastructure.Data;

namespace QRCodeBasedMetroTicketingSystem.Infrastructure.Repositories
{
    public class TransactionRepository : Repository<Transaction>, ITransactionRepository
    {
        public TransactionRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task CreateAsync(Transaction transaction)
        {
            await _dbSet.AddAsync(transaction);
        }

        public async Task<Transaction?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<Transaction?> GetByReferenceAsync(string transactionReference)
        {
            return await _dbSet.FirstOrDefaultAsync(t => t.TransactionReference == transactionReference);
        }

        public async Task<IEnumerable<Transaction>> GetByWalletIdAsync(int walletId, int limit = 10)
        {
            return await _dbSet
                .Where(t => t.WalletId == walletId)
                .OrderByDescending(t => t.CreatedAt)
                .Take(limit)
                .ToListAsync();
        }
    }
}
