using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace QRCodeBasedMetroTicketingSystem.Domain.Entities
{
    public class Trip
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int TicketId { get; set; }

        [Required]
        public int EntryStationId { get; set; }

        public int? ExitStationId { get; set; }

        [Required]
        public DateTime EntryTime { get; set; } = DateTime.UtcNow;

        public DateTime? ExitTime { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? FareAmount { get; set; }

        [Required]
        public TripStatus Status { get; set; } = TripStatus.InProgress;

        public string? TransactionReference { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public User User { get; set; }

        [ForeignKey("TicketId")]
        public Ticket Ticket { get; set; }

        [ForeignKey("EntryStationId")]
        public Station EntryStation { get; set; }

        [ForeignKey("ExitStationId")]
        public Station? ExitStation { get; set; }
    }

    public enum TripStatus
    {
        InProgress,
        Completed,
        TimedOut
    }
}
