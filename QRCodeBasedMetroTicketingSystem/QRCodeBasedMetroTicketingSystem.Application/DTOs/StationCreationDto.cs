using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace QRCodeBasedMetroTicketingSystem.Application.DTOs
{
    public class StationCreationDto : StationBaseDto
    {
        public int? InsertAfterStationId { get; set; }
        public List<StationListDto> Stations { get; set; } = new();
        public Dictionary<int, decimal> Distances { get; set; } = new();
    }
}
