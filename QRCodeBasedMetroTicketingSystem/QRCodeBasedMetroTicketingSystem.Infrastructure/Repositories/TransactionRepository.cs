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
                .Where(t => t.WalletId == walletId && t.Status != TransactionStatus.Canceled)
                .OrderByDescending(t => t.CreatedAt)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalTopUpsAsync(int userId, DateTime fromDate)
        {
            var wallet = await _context.Wallets
                .FirstOrDefaultAsync(w => w.UserId == userId);

            if (wallet == null)
                return 0;

            return await _dbSet
                .Where(t => t.WalletId == wallet.Id &&
                       t.Type == TransactionType.TopUp &&
                       t.Status == TransactionStatus.Completed &&
                       t.CreatedAt >= fromDate)
                .SumAsync(t => t.Amount);
        }

        public async Task<List<Transaction>> GetRecentTransactionsByUserIdAsync(int userId, int count = 5)
        {
            var wallet = await _context.Wallets
                .FirstOrDefaultAsync(w => w.UserId == userId);

            if (wallet == null)
                return new List<Transaction>();

            return await _context.Transactions
                .Where(t => t.WalletId == wallet.Id)
                .OrderByDescending(t => t.CreatedAt)
                .Take(count)
                .ToListAsync();
        }
    }
}
