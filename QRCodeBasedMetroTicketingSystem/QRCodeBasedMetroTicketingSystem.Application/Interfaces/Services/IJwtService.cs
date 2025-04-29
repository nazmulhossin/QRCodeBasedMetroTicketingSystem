namespace QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services
{
    public interface IJwtService
    {
        string GenerateToken(string Id, string? name, string role, string? phoneNumber = null, string? email = null);
    }
}
