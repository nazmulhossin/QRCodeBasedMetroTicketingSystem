using System.Security.Claims;

namespace QRCodeBasedMetroTicketingSystem.Application.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static int? GetUserId(this ClaimsPrincipal user)
        {
            var userId = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userId, out var id) ? id : null;
        }
    }
}
