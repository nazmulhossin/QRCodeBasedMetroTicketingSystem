using QRCodeBasedMetroTicketingSystem.Application.DTOs;
using QRCodeBasedMetroTicketingSystem.Domain.Entities;

namespace QRCodeBasedMetroTicketingSystem.Application.Interfaces.Repositories
{
    public interface ITicketRepository : IRepository<Ticket>
    {
        Task CreateTicketAsync(Ticket ticket);
        Task<Ticket?> GetTicketByIdAsync(int ticketId);
        Task<Ticket?> GetByReferenceAsync(string transactionReference);
        Task<IEnumerable<Ticket>> GetQrTicketsByStatusAsync(int userId, TicketStatus status);
        Task<int> GetActiveAndInUseTicketsCountAsync(int userId);
    }
}
