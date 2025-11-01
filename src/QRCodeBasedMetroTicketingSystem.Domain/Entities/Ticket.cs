using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace QRCodeBasedMetroTicketingSystem.Domain.Entities
{
    public class Ticket
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public TicketType Type { get; set; }

        // For QR Ticket (prepaid), these would be filled
        public int? OriginStationId { get; set; }
        public int? DestinationStationId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? FareAmount { get; set; }

        [Required]
        public string QRCodeData { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime ExpiryTime { get; set; }

        [Required]
        public TicketStatus Status { get; set; } = TicketStatus.Active;

        public string? TransactionReference { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public User User { get; set; }

        [ForeignKey("OriginStationId")]
        public Station? OriginStation { get; set; }

        [ForeignKey("DestinationStationId")]
        public Station? DestinationStation { get; set; }

        // Either associated with a Trip (for RapidPass) or can be used to create a Trip (for QRTicket)
        public Trip? Trip { get; set; }
    }

    public enum TicketType
    {
        QRTicket = 0,    // Prepaid
        RapidPass = 1    // Postpaid
    }

    public enum TicketStatus
    {
        Pending = 0,
        Active = 1,
        InUse = 2,
        Used = 3,
        Expired = 4,
        Cancelled = 5
    }
}
