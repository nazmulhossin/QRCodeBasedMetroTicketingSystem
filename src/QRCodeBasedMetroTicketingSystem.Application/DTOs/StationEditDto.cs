using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace QRCodeBasedMetroTicketingSystem.Application.DTOs
{
    public class StationEditDto : StationBaseDto
    {
        [Required]
        public int Id { get; set; }
        public List<AdjacentStationDistanceDto> Distances { get; set; } = new();
    }
}
