using System.ComponentModel.DataAnnotations;

namespace QRCodeBasedMetroTicketingSystem.Application.DTOs
{
    public class ScanTicketRequest
    {
        [Required]
        public string? QRCodeData { get; set; }

        [Required]
        public int StationId { get; set; }
    }
}
