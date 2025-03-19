namespace QRCodeBasedMetroTicketingSystem.Application.DTOs
{
    public class StationListDto
    {
        public required int StationId { get; set; }
        public required string StationName { get; set; }
        public required int Order { get; set; }
    }
}
