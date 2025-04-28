using QRCodeBasedMetroTicketingSystem.Domain.Entities;

namespace QRCodeBasedMetroTicketingSystem.Application.Interfaces.Repositories
{
    public interface ITripRepository : IRepository<Trip>
    {
        Task CreateTripAsync(Trip trip);
        Task<Trip?> GetActiveTripByTicketIdAsync(int ticketId);
        Task<IEnumerable<Trip>> GetCompletedTripsByUserIdAsync(int userId, DateTime fromDate);
        Task<int> GetTripCountByUserIdAsync(int userId, DateTime fromDate);
        Task<List<Trip>> GetRecentTripsByUserIdAsync(int userId, int count = 10);
    }
}
