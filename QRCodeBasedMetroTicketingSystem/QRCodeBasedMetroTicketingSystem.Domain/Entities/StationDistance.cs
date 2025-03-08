using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QRCodeBasedMetroTicketingSystem.Domain.Entities
{
    public class StationDistance
    {
        [Key]
        public int DistanceId { get; set; }

        // Foreign key to the Station table for the first station
        public required int Station1Id { get; set; }

        // Foreign key to the Station table for the second station
        public required int Station2Id { get; set; }

        [Column(TypeName = "decimal(12,6)")]
        public required decimal Distance { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties to related Station entities
        public virtual Station? Station1 { get; set; }
        public virtual Station? Station2 { get; set; }
    }
}
