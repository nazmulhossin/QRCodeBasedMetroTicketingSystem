using QRCodeBasedMetroTicketingSystem.Application.DTOs;
using System.ComponentModel.DataAnnotations;

namespace QRCodeBasedMetroTicketingSystem.Web.Areas.System.ViewModels
{
    public class SelectStationViewModel
    {
        [Required(ErrorMessage = "Please select a station")]
        public int? StationId { get; set; }
        public string? StationName { get; set; }
        public List<StationSummaryDto> StationList { get; set; } = [];
    }
}
