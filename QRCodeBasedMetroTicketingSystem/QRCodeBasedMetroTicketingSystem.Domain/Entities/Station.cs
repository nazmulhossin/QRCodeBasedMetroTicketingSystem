using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QRCodeBasedMetroTicketingSystem.Domain.Entities
{
    public class Station
    {
        [Key]
        public int StationId { get; set; }

        [Required, StringLength(100)]
        public string? StationName { get; set; }

        [Required, StringLength(255)]
        public string? Address { get; set; }

        [Required]
        public decimal Latitude { get; set; }

        [Required]
        public decimal Longitude { get; set; }

        [Required]
        public int Order { get; set; }

        [Required, StringLength(10)]
        [RegularExpression("Active|Inactive", ErrorMessage = "Status must be 'Active' or 'Inactive'.")]
        public string Status { get; set; } = "Active";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
