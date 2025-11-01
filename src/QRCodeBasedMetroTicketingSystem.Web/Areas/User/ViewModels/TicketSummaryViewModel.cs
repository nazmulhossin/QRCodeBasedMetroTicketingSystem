using System.ComponentModel.DataAnnotations;

namespace QRCodeBasedMetroTicketingSystem.Web.Areas.User.ViewModels
{
    public class TicketSummaryViewModel
    {
        [Required]
        public int OriginStationId { get; set; }

        [Required]
        public int DestinationStationId { get; set; }

        public string? OriginStationName { get; set; }
        public string? DestinationStationName { get; set; }
        public decimal Fare { get; set; }
        public string TicketType { get; set; } = "Single Journey";
    }
}
