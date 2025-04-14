using Microsoft.EntityFrameworkCore;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Repositories;
using QRCodeBasedMetroTicketingSystem.Domain.Entities;
using QRCodeBasedMetroTicketingSystem.Infrastructure.Data;

namespace QRCodeBasedMetroTicketingSystem.Infrastructure.Repositories
{
    public class WalletRepository : Repository<Wallet>, IWalletRepository
    {
        public WalletRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Wallet?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<Wallet?> GetByUserIdAsync(int userId)
        {
            return await _dbSet.FirstOrDefaultAsync(w => w.UserId == userId);
        }

        public async Task CreateAsync(Wallet wallet)
        {
            await _context.Wallets.AddAsync(wallet);
        }
    }
}
