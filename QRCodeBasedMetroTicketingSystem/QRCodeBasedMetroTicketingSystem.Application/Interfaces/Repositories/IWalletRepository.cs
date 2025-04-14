using QRCodeBasedMetroTicketingSystem.Domain.Entities;

namespace QRCodeBasedMetroTicketingSystem.Application.Interfaces.Repositories
{
    public interface IWalletRepository : IRepository<Wallet>
    {
        Task<Wallet?> GetByIdAsync(int id);
        Task<Wallet?> GetByUserIdAsync(int userId);
        Task CreateAsync(Wallet wallet);
    }
}
