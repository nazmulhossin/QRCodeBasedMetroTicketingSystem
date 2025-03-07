using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace QRCodeBasedMetroTicketingSystem.Web.Areas.Admin.ViewModels
{
    public class StationViewModel
    {
        [Key]
        public int StationId { get; set; }

        [Required, StringLength(100)]
        public string? StationName { get; set; }

        [Required, StringLength(255)]
        public string? Address { get; set; }

        [Required, Column(TypeName = "decimal(10,8)")]
        public decimal Latitude { get; set; }

        [Required, Column(TypeName = "decimal(11,8)")]
        public decimal Longitude { get; set; }

        public required int Order { get; set; }

        [Required, StringLength(10)]
        [RegularExpression("Active|Inactive", ErrorMessage = "Status must be 'Active' or 'Inactive'.")]
        public string Status { get; set; } = "Active";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
