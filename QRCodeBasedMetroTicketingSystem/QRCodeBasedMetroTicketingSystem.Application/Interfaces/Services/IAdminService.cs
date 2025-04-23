namespace QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services
{
    public interface IAdminService
    {
        Task<(bool Success, int AdminId, string? Token)> ValidateAdminCredentialsAsync(string email, string password);
    }
}
