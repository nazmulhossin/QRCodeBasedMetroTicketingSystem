namespace QRCodeBasedMetroTicketingSystem.Application.DTOs
{
    public class TripSummaryDto
    {
        public string? EntryStationName { get; set; }
        public string? ExitStationName { get; set; }
        public string? EntryTime { get; set; }
        public string? ExitTime { get; set; }
        public decimal FareAmount { get; set; }
    }
}
