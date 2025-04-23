using Microsoft.EntityFrameworkCore;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Repositories;
using QRCodeBasedMetroTicketingSystem.Domain.Entities;
using QRCodeBasedMetroTicketingSystem.Infrastructure.Data;

namespace QRCodeBasedMetroTicketingSystem.Infrastructure.Repositories
{
    public class AdminRepository : Repository<Admin>, IAdminRepository
    {
        public AdminRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Admin?> GetByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(a => a.Email == email);
        }
    }
}
