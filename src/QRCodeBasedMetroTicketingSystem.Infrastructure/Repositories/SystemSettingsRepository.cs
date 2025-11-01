using Microsoft.EntityFrameworkCore;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Repositories;
using QRCodeBasedMetroTicketingSystem.Domain.Entities;
using QRCodeBasedMetroTicketingSystem.Infrastructure.Data;

namespace QRCodeBasedMetroTicketingSystem.Infrastructure.Repositories
{
    public class SystemSettingsRepository : Repository<SystemSettings>, ISystemSettingsRepository
    {
        public SystemSettingsRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<SystemSettings?> GetSystemSettingsAsync()
        {
            return await _dbSet.FirstOrDefaultAsync();
        }

        public async Task UpdateSettingsAsync(SystemSettings settings)
        {
            _dbSet.Update(settings);
        }
    }
}
