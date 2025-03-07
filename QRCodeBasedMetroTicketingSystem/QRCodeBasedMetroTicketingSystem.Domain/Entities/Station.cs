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
        public long StationId { get; set; }

        [StringLength(100)]
        public required string StationName { get; set; }

        [StringLength(255)]
        public required string Address { get; set; }

        [Column(TypeName = "decimal(10,8)")]
        public required decimal Latitude { get; set; }

        [Column(TypeName = "decimal(11,8)")]
        public required decimal Longitude { get; set; }

        public required int Order { get; set; }

        [StringLength(10)]
        public required string Status { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
