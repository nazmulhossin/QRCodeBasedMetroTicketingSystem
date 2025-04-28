using QRCodeBasedMetroTicketingSystem.Application.DTOs;

namespace QRCodeBasedMetroTicketingSystem.Web.Areas.User.ViewModels
{
    public class UserDashboardViewModel
    {
        public UserDashboardStatsDto Stats { get; set; }
        public List<UserActivityDto> Activities { get; set; }
    }
}
