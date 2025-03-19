using System.ComponentModel.DataAnnotations;

namespace QRCodeBasedMetroTicketingSystem.Application.DTOs
{
    public class StationDto : StationBaseDto
    {
        [Required]
        public int StationId { get; set; }

        [Required]
        public int Order { get; set; }
    }
}
