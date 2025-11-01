namespace QRCodeBasedMetroTicketingSystem.Application.DTOs
{
    public class FareDistanceDto
    {
        public string? FromStationName { get; set; }
        public string? ToStationName { get; set; }
        public decimal Distance { get; set; }
        public int Fare { get; set; }
    }
}
