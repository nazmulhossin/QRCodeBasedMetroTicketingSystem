using System.ComponentModel.DataAnnotations;

namespace QRCodeBasedMetroTicketingSystem.Web.Areas.User.ViewModels
{
    public class TicketPaymentViewModel
    {
        [Required]
        public int OriginStationId { get; set; }

        [Required]
        public int DestinationStationId { get; set; }

        [Required]
        public decimal FareAmount { get; set; }

        [Required]
        public string PaymentMethod { get; set; } // "AccountBalance" or "OnlinePayment"

        public string? OriginStationName { get; set; }
        public string? DestinationStationName { get; set; }
    }
}
