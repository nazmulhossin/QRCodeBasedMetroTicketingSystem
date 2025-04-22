using System.ComponentModel.DataAnnotations;

namespace QRCodeBasedMetroTicketingSystem.Web.Areas.User.ViewModels
{
    public class PaymentOptionViewModel
    {
        [Required]
        public int OriginStationId { get; set; }

        [Required]
        public int DestinationStationId { get; set; }

        [Required]
        public PaymentOption PaymentOption { get; set; }
    }

    public enum PaymentOption 
    {
        AccountBalance,
        OnlinePayment
    }
}
