namespace QRCodeBasedMetroTicketingSystem.Application.DTOs
{
    public class StationDto
    {
        public int StationId { get; set; }
        public required string StationName { get; set; }
        public required string Address { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public required int Order { get; set; }
        public required string Status { get; set; }
    }
}
