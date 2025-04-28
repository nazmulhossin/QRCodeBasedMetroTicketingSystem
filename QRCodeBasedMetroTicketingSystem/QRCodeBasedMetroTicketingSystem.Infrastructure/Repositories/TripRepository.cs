using Microsoft.EntityFrameworkCore;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Repositories;
using QRCodeBasedMetroTicketingSystem.Domain.Entities;
using QRCodeBasedMetroTicketingSystem.Infrastructure.Data;

namespace QRCodeBasedMetroTicketingSystem.Infrastructure.Repositories
{
    public class TripRepository : Repository<Trip>, ITripRepository
    {
        public TripRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task CreateTripAsync(Trip trip)
        {
            await _dbSet.AddAsync(trip);
        }

        public async Task<Trip?> GetActiveTripByTicketIdAsync(int ticketId)
        {
            return await _dbSet
                .AsSplitQuery()
                .Include(t => t.EntryStation)
                .FirstOrDefaultAsync(t =>
                    t.TicketId == ticketId &&
                    t.Status == TripStatus.InProgress);
        }

        public async Task<IEnumerable<Trip>> GetCompletedTripsByUserIdAsync(int userId, DateTime fromDate)
        {
            return await _dbSet
                .AsSplitQuery()
                .Include(t => t.EntryStation)
                .Include(t => t.ExitStation)
                .Include(t => t.Ticket)
                .Where(t => t.UserId == userId
                            && t.Status == TripStatus.Completed
                            && t.ExitTime >= fromDate)
                .OrderByDescending(t => t.ExitTime)
                .ToListAsync();
        }

        public async Task<int> GetTripCountByUserIdAsync(int userId, DateTime fromDate)
        {
            return await _dbSet.CountAsync(t => t.UserId == userId && t.EntryTime >= fromDate);
        }

        public async Task<List<Trip>> GetRecentTripsByUserIdAsync(int userId, int count = 10)
        {
            return await _dbSet
                .AsSplitQuery()
                .Where(t => t.UserId == userId)
                .Include(t => t.EntryStation)
                .Include(t => t.ExitStation)
                .Include(t => t.Ticket)
                .OrderByDescending(t => t.EntryTime)
                .Take(count)
                .ToListAsync();
        }
    }
}
