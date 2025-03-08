using Microsoft.AspNetCore.Mvc.Rendering;
using QRCodeBasedMetroTicketingSystem.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QRCodeBasedMetroTicketingSystem.Web.Areas.Admin.ViewModels
{
    public class StationCreationViewModel
    {
        [Required, StringLength(100)]
        public string? StationName { get; set; }

        [Required, StringLength(255)]
        public string? Address { get; set; }

        [Required]
        [Column(TypeName = "decimal(12,8)")]
        [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90")]
        public decimal? Latitude { get; set; }

        [Required]
        [Column(TypeName = "decimal(12,8)")]
        [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180")]
        public decimal? Longitude { get; set; }

        [Required, StringLength(50)]
        public string? Status { get; set; }

        public int? InsertAfterStationId { get; set; }
        public List<StationListViewModel>? Stations { get; set; } = new();
        public Dictionary<int, decimal>? Distances { get; set; } = new();
    }
}
