using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace QRCodeBasedMetroTicketingSystem.Application.DTOs
{
    public class SettingsDto
    {
        [Required(ErrorMessage = "Minimum fare is required")]
        [Display(Name = "Minimum Fare (TK)")]
        [Range(0, double.MaxValue, ErrorMessage = "Minimum fare must not be negative")]
        public decimal MinFare { get; set; }

        [Required(ErrorMessage = "Maximum fare is required")]
        [Display(Name = "Maximum Fare (TK)")]
        [Range(0, double.MaxValue, ErrorMessage = "Maximum fare must not be negative")]
        public decimal MaxFare { get; set; }

        [Required(ErrorMessage = "Fare per kilometer is required")]
        [Display(Name = "Fare Per Kilometer (TK)")]
        [Range(0, double.MaxValue, ErrorMessage = "Fare per kilometer must not be negative")]
        public decimal FarePerKm { get; set; }

        [Required(ErrorMessage = "QR code validity time is required")]
        [Display(Name = "QR Code Valid Time (Minutes)")]
        [Range(1, int.MaxValue, ErrorMessage = "QR code validity time must be at least 1 minute")]
        public int QrCodeValidTime { get; set; }
    }
}
