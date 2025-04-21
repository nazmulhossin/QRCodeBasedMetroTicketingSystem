using QRCodeBasedMetroTicketingSystem.Application.DTOs;
using QRCodeBasedMetroTicketingSystem.Domain.Entities;

namespace QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services
{
    public interface IPaymentService
    {
        Task<string> InitiatePaymentAsync(int userId, decimal amount, PaymentMethod paymentMethod, string? transactionReference);
        Task<bool> VerifyPaymentAsync(string transactionReference, PaymentMethod paymentMethod);
        Task<bool> CancelPaymentAsync(string transactionReference);
        Task<TransactionDto?> GetTransactionByReferenceAsync(string transactionReference);
    }
}
