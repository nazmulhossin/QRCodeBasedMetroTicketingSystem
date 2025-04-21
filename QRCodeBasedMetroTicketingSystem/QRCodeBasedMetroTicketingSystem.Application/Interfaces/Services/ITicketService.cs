using QRCodeBasedMetroTicketingSystem.Application.DTOs;

namespace QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services
{
    public interface ITicketService
    {
        Task<(string OriginStationName, string DestinationStationName, int Fare)> GetTicketSummaryAsync(int fromStationId, int toStationId);
        Task<(string TransactionReference, decimal Amount)> InitiatePurchaseQRTicketAsync(int userId, int originStationId, int destinationStationId, string paymentMethod);
        Task<bool> CompleteQRTicketPurchaseAsync(string transactionReference);
        Task<TicketDto?> GetTicketByReferenceAsync(string transactionReference);
    }
}
