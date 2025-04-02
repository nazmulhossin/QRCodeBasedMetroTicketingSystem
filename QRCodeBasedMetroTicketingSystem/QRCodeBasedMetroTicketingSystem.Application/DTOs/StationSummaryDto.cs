namespace QRCodeBasedMetroTicketingSystem.Application.DTOs
{
    public class StationSummaryDto
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required int Order { get; set; }
    }
}
