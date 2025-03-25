using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QRCodeBasedMetroTicketingSystem.Domain.Entities
{
    public class Station
    {
        public int Id { get; set; }

        [StringLength(100)]
        public required string Name { get; set; }

        [StringLength(255)]
        public required string Address { get; set; }

        [Column(TypeName = "decimal(12,8)")]
        public required decimal Latitude { get; set; }

        [Column(TypeName = "decimal(12,8)")]
        public required decimal Longitude { get; set; }

        public required int Order { get; set; }

        [StringLength(50)]
        public required string Status { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
