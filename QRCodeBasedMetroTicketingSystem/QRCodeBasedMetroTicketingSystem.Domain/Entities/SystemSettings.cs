using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QRCodeBasedMetroTicketingSystem.Domain.Entities
{
    public class SystemSettings
    {
        public int Id { get; set; }

        [Range(0, double.MaxValue)]
        public decimal MinFare { get; set; } = 20.0000m;

        [Range(0, double.MaxValue)]
        public decimal FarePerKm { get; set; } = 5.0000m;

        [Range(1, int.MaxValue)]
        public int RapidPassQrCodeValidityMinutes { get; set; } = 1440;

        [Range(1, int.MaxValue)]
        public int QrTicketValidityMinutes { get; set; } = 2880;

        [Range(1, int.MaxValue)]
        public int MaxTripDurationMinutes { get; set; } = 120;

        [Range(0, double.MaxValue)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TimeLimitPenaltyFee { get; set; } = 100.00m;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
