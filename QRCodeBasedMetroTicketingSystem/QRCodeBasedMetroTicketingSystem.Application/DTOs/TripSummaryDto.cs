namespace QRCodeBasedMetroTicketingSystem.Application.DTOs
{
    public class TripSummaryDto
    {
        public string? OriginStationName { get; set; }
        public string? DestinationStationName { get; set; }
        public DateTime EntryTime { get; set; }
        public DateTime ExitTime { get; set; }
        public decimal FareAmount { get; set; }
    }
}
