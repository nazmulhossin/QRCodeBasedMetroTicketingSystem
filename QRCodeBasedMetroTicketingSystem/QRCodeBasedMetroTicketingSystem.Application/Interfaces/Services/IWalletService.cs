using QRCodeBasedMetroTicketingSystem.Application.DTOs;
using QRCodeBasedMetroTicketingSystem.Domain.Entities;

namespace QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services
{
    public interface IWalletService
    {
        Task<WalletDto> GetWalletByUserIdAsync(int userId);
        Task<decimal> GetBalanceAsync(int userId);
        Task<bool> AddBalanceAsync(int userId, decimal amount, PaymentMethod paymentMethod, string transactionReference);
        Task<bool> DeductBalanceAsync(int userId, decimal amount, TransactionType type, string description);
    }
}
