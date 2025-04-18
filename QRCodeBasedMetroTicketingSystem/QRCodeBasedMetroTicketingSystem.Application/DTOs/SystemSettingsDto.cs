using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace QRCodeBasedMetroTicketingSystem.Application.DTOs
{
    public class SystemSettingsDto
    {
        [Required(ErrorMessage = "Minimum fare is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Minimum fare must not be negative")]
        public decimal MinFare { get; set; }

        [Required(ErrorMessage = "Fare per kilometer is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Fare per kilometer must not be negative")]
        public decimal FarePerKm { get; set; }

        [Required(ErrorMessage = "Rapid Pass QR code validity time is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Rapid Pass QR code validity time must be at least 1 minute")]
        public int RapidPassQrCodeValidityMinutes { get; set; } = 1440;

        [Required(ErrorMessage = "QR ticket validity time is required")]
        [Range(1, int.MaxValue, ErrorMessage = "QR ticket validity time must be at least 1 minute")]
        public int QrTicketValidityMinutes { get; set; } = 2880;

        [Required(ErrorMessage = "Maximum Trip Duration is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Maximum Trip Duration must be at least 1 minute")]
        public int MaxTripDurationMinutes { get; set; } = 120;

        [Required(ErrorMessage = "Time Limit Penalty Fee is required")]
        [Range(0, double.MaxValue)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TimeLimitPenaltyFee { get; set; } = 100.00m;
    }
}
