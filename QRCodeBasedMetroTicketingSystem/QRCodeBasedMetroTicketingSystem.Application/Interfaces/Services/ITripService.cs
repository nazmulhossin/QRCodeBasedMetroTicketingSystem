using QRCodeBasedMetroTicketingSystem.Application.DTOs;

namespace QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services
{
    public interface ITripService
    {
        Task<List<TripDto>> GetCompletedTripsByUserIdAsync(int userId);
    }
}
