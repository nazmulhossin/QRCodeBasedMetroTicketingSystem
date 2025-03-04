namespace QRCodeBasedMetroTicketingSystem.Web.Areas.Admin.ViewModels
{
    public class StationViewModel
    {
        public int StationId { get; set; }
        public string? StationCode { get; set; }
        public string? StationName { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string? Address { get; set; }
        public string? Status { get; set; }
    }
}
