namespace QRCodeBasedMetroTicketingSystem.Application.DTOs
{
    public class AdjacentStationDistanceDto
    {
        public required int StationId { get; set; }
        public required int AdjacentStationId { get; set; }
        public string StationName { get; set; } = string.Empty;
        public required decimal Distance { get; set; }
    }
}
