using QRCodeBasedMetroTicketingSystem.Domain.Entities;

namespace QRCodeBasedMetroTicketingSystem.Application.Interfaces.Repositories
{
    public interface ISystemSettingsRepository : IRepository<SystemSettings>
    {
        Task<SystemSettings?> GetSystemSettingsAsync();
        Task UpdateSettingsAsync(SystemSettings settings);
    }
}
