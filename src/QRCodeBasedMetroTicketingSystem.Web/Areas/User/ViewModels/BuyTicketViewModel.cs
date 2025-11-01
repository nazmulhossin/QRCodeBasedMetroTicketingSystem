using System.ComponentModel.DataAnnotations;

namespace QRCodeBasedMetroTicketingSystem.Web.Areas.User.ViewModels
{
    public class BuyTicketViewModel
    {
        [Required]
        public int OriginStationId { get; set; }

        [Required]
        public int DestinationStationId { get; set; }
    }
}
