using QRCodeBasedMetroTicketingSystem.Domain.Entities;

namespace QRCodeBasedMetroTicketingSystem.Web.Areas.User.ViewModels
{
    public class TicketViewModel
    {
        public int Id { get; set; }
        public string OriginStationName { get; set; }
        public string DestinationStationName { get; set; }
        public decimal FareAmount { get; set; }
        public TicketStatus Status { get; set; }
        public DateTime ExpiryTime { get; set; }
    }
}
