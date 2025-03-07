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

        [Required]
        public int Station1Id { get; set; }

        [Required]
        public int Station2Id { get; set; }

        [Required]
        public decimal Distance { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public Station? Station1 { get; set; }
        public Station? Station2 { get; set; }
    }
}
