using QRCodeBasedMetroTicketingSystem.Domain.Entities;

namespace QRCodeBasedMetroTicketingSystem.Application.Interfaces.Repositories
{
    public interface ITripRepository : IRepository<Trip>
    {
        Task CreateTripAsync(Trip trip);
        Task<Trip?> GetActiveTripByTicketIdAsync(int ticketId);
    }
}
