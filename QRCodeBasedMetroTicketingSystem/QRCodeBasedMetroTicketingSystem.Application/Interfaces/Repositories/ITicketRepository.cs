using QRCodeBasedMetroTicketingSystem.Domain.Entities;

namespace QRCodeBasedMetroTicketingSystem.Application.Interfaces.Repositories
{
    public interface ITicketRepository : IRepository<Ticket>
    {
        Task CreateTicketAsync(Ticket ticket);
        Task<Ticket?> GetByReferenceAsync(string transactionReference);
        Task<IEnumerable<Ticket>> GetQrTicketsByStatusAsync(int userId, TicketStatus status);
        Task<int> GetActiveAndInUseTicketsCountAsync(int userId);
    }
}
