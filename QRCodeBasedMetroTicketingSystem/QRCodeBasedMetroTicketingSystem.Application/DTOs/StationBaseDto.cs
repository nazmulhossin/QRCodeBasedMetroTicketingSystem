using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace QRCodeBasedMetroTicketingSystem.Application.DTOs
{
    public class StationBaseDto
    {
        [Required, StringLength(100)]
        public string StationName { get; set; } = string.Empty;

        [Required, StringLength(255)]
        public string Address { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(12,8)")]
        [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90")]
        public decimal? Latitude { get; set; }

        [Required]
        [Column(TypeName = "decimal(12,8)")]
        [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180")]
        public decimal? Longitude { get; set; }

        [Required, StringLength(50)]
        public string Status { get; set; } = string.Empty;
    }
}
