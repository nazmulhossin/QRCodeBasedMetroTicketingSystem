using QRCodeBasedMetroTicketingSystem.Application.DTOs;

namespace QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services
{
    public interface IUserDashboardService
    {
        Task<UserDashboardStatsDto> GetUserStatsAsync(int userId);
        Task<List<UserActivityDto>> GetRecentActivitiesAsync(int userId, int count = 5);
    }
}
