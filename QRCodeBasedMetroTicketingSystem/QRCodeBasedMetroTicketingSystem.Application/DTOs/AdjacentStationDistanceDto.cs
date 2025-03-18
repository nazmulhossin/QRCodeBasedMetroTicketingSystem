namespace QRCodeBasedMetroTicketingSystem.Application.DTOs
{
    public class AdjacentStationDistanceDto
    {
        public int StationId { get; set; }
        public int AdjacentStationId { get; set; }
        public string StationName { get; set; } = string.Empty;
        public decimal Distance { get; set; }
    }
}
