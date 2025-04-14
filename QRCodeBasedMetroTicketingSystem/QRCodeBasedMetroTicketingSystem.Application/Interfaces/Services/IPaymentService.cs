using QRCodeBasedMetroTicketingSystem.Domain.Entities;

namespace QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services
{
    public interface IPaymentService
    {
        Task<string> InitiatePaymentAsync(int userId, decimal amount, PaymentMethod paymentMethod);
        Task<bool> VerifyPaymentAsync(string transactionReference, PaymentMethod paymentMethod);
    }
}
