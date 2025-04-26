using QRCodeBasedMetroTicketingSystem.Application.DTOs;
using QRCodeBasedMetroTicketingSystem.Domain.Entities;

namespace QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services
{
    public interface ITicketService
    {
        Task<IEnumerable<TicketDto>> GetQrTicketsByStatusAsync(int userId, TicketStatus status);
        Task<TicketDto?> GetTicketByIdAsync(int ticketId);
        Task<int> GetActiveAndInUseTicketsCountAsync(int userId);
        Task<TicketDto?> GetActiveRapidPassTicketByUserIdAsync(int userId);
        Task<(string OriginStationName, string DestinationStationName, int Fare)> GetTicketSummaryAsync(int fromStationId, int toStationId);
        Task<(string TransactionReference, decimal Amount)> InitiatePurchaseQRTicketAsync(int userId, int originStationId, int destinationStationId, string paymentMethod);
        Task<bool> CompleteQRTicketPurchaseAsync(string transactionReference);
        Task<TicketDto?> GetTicketByReferenceAsync(string transactionReference);
    }
}
