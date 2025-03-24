using Microsoft.EntityFrameworkCore;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Repositories;
using QRCodeBasedMetroTicketingSystem.Domain.Entities;
using QRCodeBasedMetroTicketingSystem.Infrastructure.Data;

namespace QRCodeBasedMetroTicketingSystem.Infrastructure.Repositories
{
    public class SettingsRepository : Repository<Settings>, ISettingsRepository
    {
        public SettingsRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Settings?> GetCurrentSettingsAsync()
        {
            return await _dbSet.FirstOrDefaultAsync();
        }

        public async Task UpdateSettingsAsync(Settings settings)
        {
            _dbSet.Update(settings);
        }
    }
}
