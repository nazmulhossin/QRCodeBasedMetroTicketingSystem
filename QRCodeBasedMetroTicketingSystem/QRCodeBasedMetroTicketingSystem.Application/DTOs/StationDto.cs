namespace QRCodeBasedMetroTicketingSystem.Application.DTOs
{
    public class StationDto
    {
        public required int StationId { get; set; }
        public string? StationName { get; set; }
        public string? Address { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public required int Order { get; set; }
        public string? Status { get; set; }
    }
}
