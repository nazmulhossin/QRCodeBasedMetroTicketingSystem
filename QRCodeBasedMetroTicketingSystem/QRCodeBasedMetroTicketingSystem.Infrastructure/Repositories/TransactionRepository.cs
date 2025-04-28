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

        public async Task<List<Transaction>> GetRecentTransactionsByUserIdAsync(int userId, int count = 10)
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

        public async Task<IDictionary<string, decimal>> GetMonthlyRevenueAsync(int months)
        {
            DateTime startDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1).AddMonths(-months + 1);

            // First, get the raw data without string formatting in the query
            var rawData = await _dbSet
                .Where(t => t.CreatedAt >= startDate &&
                           t.Status == TransactionStatus.Completed && t.Type == TransactionType.Payment)
                .GroupBy(t => new { t.CreatedAt.Month, t.CreatedAt.Year })
                .Select(g => new {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Revenue = g.Sum(t => t.Amount)
                })
                .ToListAsync();

            // Then format the string after retrieving the data
            var result = rawData
                .Select(x => new {
                    MonthYear = $"{x.Year}-{x.Month:D2}",
                    Revenue = x.Revenue
                })
                .OrderBy(x => x.MonthYear)
                .ToDictionary(x => x.MonthYear, x => x.Revenue);

            // Fill in any missing months with zero revenue
            for (int i = 0; i < months; i++)
            {
                DateTime date = startDate.AddMonths(i);
                string monthYear = $"{date.Year}-{date.Month:D2}";
                if (!result.ContainsKey(monthYear))
                {
                    result[monthYear] = 0;
                }
            }

            return result.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
        }

        public async Task<decimal> GetTotalRevenueAsync()
        {
            return await _dbSet
                .Where(t => t.Status == TransactionStatus.Completed && t.Type == TransactionType.Payment)
                .SumAsync(t => t.Amount);
        }
    }
}
