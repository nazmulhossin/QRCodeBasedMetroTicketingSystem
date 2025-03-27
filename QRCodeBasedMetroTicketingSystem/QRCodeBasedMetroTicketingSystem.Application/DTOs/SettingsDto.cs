using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace QRCodeBasedMetroTicketingSystem.Application.DTOs
{
    public class SettingsDto
    {
        [Required(ErrorMessage = "Minimum fare is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Minimum fare must not be negative")]
        public decimal MinFare { get; set; }

        [Required(ErrorMessage = "Fare per kilometer is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Fare per kilometer must not be negative")]
        public decimal FarePerKm { get; set; }

        [Required(ErrorMessage = "QR code validity time is required")]
        [Range(1, int.MaxValue, ErrorMessage = "QR code validity time must be at least 1 minute")]
        public int QrCodeValidTime { get; set; }

        [Required(ErrorMessage = "QR code ticket validity time is required")]
        [Range(1, int.MaxValue, ErrorMessage = "QR code ticket validity time must be at least 1 minute")]
        public int QrCodeTicketValidTime { get; set; }

        [Required(ErrorMessage = "Trip Time Limit is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Trip Time Limit must be at least 1 minute")]
        public int TripTimeLimit { get; set; }
    }
}
