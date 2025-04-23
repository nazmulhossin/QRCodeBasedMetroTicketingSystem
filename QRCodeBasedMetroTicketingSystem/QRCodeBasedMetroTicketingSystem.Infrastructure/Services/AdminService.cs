using Microsoft.EntityFrameworkCore;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Repositories;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services;

namespace QRCodeBasedMetroTicketingSystem.Infrastructure.Services
{
    public class AdminService : IAdminService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtService _jwtService;
        private const string role = "Admin";

        public AdminService(IUnitOfWork unitOfWork, IJwtService jwtService)
        {
            _unitOfWork = unitOfWork;
            _jwtService = jwtService;
        }

        public async Task<(bool Success, int AdminId, string? Token)> ValidateAdminCredentialsAsync(string email, string password)
        {
            var admin = await _unitOfWork.AdminRepository.GetByEmailAsync(email);
            if (admin == null || !BCrypt.Net.BCrypt.Verify(password, admin.PasswordHash))
            {
                return (false, 0, null);
            }

            // Generate JWT token
            var token = _jwtService.GenerateToken(admin.Id.ToString(), admin.Email, role);

            // Update last login time
            admin.LastLoginAt = DateTime.UtcNow;
            await _unitOfWork.SaveChangesAsync();

            return (true, admin.Id, token);
        }
    }
}
