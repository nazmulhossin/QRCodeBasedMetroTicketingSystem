namespace QRCodeBasedMetroTicketingSystem.Application.DTOs
{
    public class ScanTicketResponseDto
    {
        public bool IsValid { get; set; }
        public string? Message { get; set; }
        public TripSummaryDto? TripSummary { get; set; }
    }
}
