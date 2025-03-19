using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace QRCodeBasedMetroTicketingSystem.Application.DTOs
{
    public class StationEditDto
    {
        public required int StationId { get; set; }

        [StringLength(100)]
        public required string StationName { get; set; }

        [StringLength(255)]
        public required string Address { get; set; }

        [Column(TypeName = "decimal(12,8)")]
        [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90")]
        public required decimal Latitude { get; set; }

        [Column(TypeName = "decimal(12,8)")]
        [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180")]
        public required decimal Longitude { get; set; }

        [StringLength(50)]
        public required string Status { get; set; }

        public List<AdjacentStationDistanceDto>? Distances { get; set; } = new();
    }
}
