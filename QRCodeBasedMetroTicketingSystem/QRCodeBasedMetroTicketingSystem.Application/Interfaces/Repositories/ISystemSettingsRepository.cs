using QRCodeBasedMetroTicketingSystem.Domain.Entities;

namespace QRCodeBasedMetroTicketingSystem.Application.Interfaces.Repositories
{
    public interface ISystemSettingsRepository : IRepository<SystemSettings>
    {
        Task<SystemSettings?> GetCurrentSettingsAsync();
        Task UpdateSettingsAsync(SystemSettings settings);
    }
}
