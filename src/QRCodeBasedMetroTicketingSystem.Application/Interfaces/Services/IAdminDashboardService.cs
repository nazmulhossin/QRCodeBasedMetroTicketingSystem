using QRCodeBasedMetroTicketingSystem.Application.DTOs;

namespace QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services
{
    public interface IAdminDashboardService
    {
        Task<AdminDashboardDto> GetDashboardDataAsync();
    }
}
