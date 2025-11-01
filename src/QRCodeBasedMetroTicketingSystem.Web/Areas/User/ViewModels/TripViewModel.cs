namespace QRCodeBasedMetroTicketingSystem.Web.Areas.User.ViewModels
{
    public class TripViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string? EntryStationName { get; set; }
        public string? ExitStationName { get; set; }
        public string? EntryTimeFormatted { get; set; }
        public string? ExitTimeFormatted { get; set; }
        public string? TicketType { get; set; }
        public decimal Fare { get; set; }
    }
}
