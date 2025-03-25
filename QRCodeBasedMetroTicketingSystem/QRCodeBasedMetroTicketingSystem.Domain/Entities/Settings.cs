using System.ComponentModel.DataAnnotations;

namespace QRCodeBasedMetroTicketingSystem.Domain.Entities
{
    public class Settings
    {
        [Key]
        public int Id { get; set; }

        [Range(0, double.MaxValue)]
        public decimal MinFare { get; set; } = 20.0000m;

        [Range(0, double.MaxValue)]
        public decimal FarePerKm { get; set; } = 5.0000m;

        [Range(0, int.MaxValue)]
        public int QrCodeValidTime { get; set; } = 1440;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
