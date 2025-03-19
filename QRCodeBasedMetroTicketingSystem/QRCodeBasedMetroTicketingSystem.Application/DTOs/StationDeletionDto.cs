using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace QRCodeBasedMetroTicketingSystem.Application.DTOs
{
    public class StationDeletionDto : StationBaseDto
    {
        [Required]
        public int StationId { get; set; }
        public List<AdjacentStationDistanceDto> Distances { get; set; } = new();
    }
}
