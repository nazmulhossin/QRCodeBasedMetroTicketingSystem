using QRCodeBasedMetroTicketingSystem.Domain.Entities;

namespace QRCodeBasedMetroTicketingSystem.Application.Interfaces.Repositories
{
    public interface IAdminRepository : IRepository<Admin>
    {
        Task<Admin?> GetByEmailAsync(string email);
    }
}
