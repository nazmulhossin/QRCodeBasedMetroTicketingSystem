using QRCodeBasedMetroTicketingSystem.Domain.Entities;

namespace QRCodeBasedMetroTicketingSystem.Application.DTOs
{
    public class TicketDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public TicketType Type { get; set; }
        public int? OriginStationId { get; set; }
        public string? OriginStationName { get; set; }
        public int? DestinationStationId { get; set; }
        public string? DestinationStationName { get; set; }
        public decimal? FareAmount { get; set; }
        public required string QRCodeData { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiryTime { get; set; }
        public TicketStatus Status { get; set; }
        public string? TransactionReference { get; set; }
    }
}
