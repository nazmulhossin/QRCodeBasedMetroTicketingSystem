using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace QRCodeBasedMetroTicketingSystem.Web.Areas.Admin.ViewModels
{
    public class StationViewModel
    {
        public required string StationName { get; set; }
        public required string Address { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public required int Order { get; set; }
        public required string Status { get; set; }
    }
}
