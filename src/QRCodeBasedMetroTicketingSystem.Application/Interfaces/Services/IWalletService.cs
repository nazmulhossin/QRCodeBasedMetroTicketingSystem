using QRCodeBasedMetroTicketingSystem.Application.DTOs;
using QRCodeBasedMetroTicketingSystem.Domain.Entities;

namespace QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services
{
    public interface IWalletService
    {
        Task<WalletDto> GetWalletByUserIdAsync(int userId);
        Task<decimal> GetBalanceByUserIdAsync(int userId);
        Task<bool> DeductBalanceAsync(int userId, decimal amount);
    }
}
