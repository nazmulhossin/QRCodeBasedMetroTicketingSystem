using QRCodeBasedMetroTicketingSystem.Domain.Entities;

namespace QRCodeBasedMetroTicketingSystem.Application.DTOs
{
    public class TripDto
    {
        public int Id { get; set; }
        public string? EntryStationName { get; set; }
        public string? ExitStationName { get; set; }
        public DateTime EntryTime { get; set; }
        public DateTime ExitTime { get; set; }
        public TicketType TicketType { get; set; }
        public decimal FareAmount { get; set; }
    }
}
