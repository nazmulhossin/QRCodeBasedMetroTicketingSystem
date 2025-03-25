using QRCodeBasedMetroTicketingSystem.Domain.Entities;

namespace QRCodeBasedMetroTicketingSystem.Application.Interfaces.Repositories
{
    public interface ISettingsRepository : IRepository<Settings>
    {
        Task<Settings?> GetCurrentSettingsAsync();
        Task UpdateSettingsAsync(Settings settings);
    }
}
