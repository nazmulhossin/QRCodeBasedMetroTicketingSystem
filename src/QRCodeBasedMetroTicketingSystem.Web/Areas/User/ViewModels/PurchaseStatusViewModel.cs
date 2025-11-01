namespace QRCodeBasedMetroTicketingSystem.Web.Areas.User.ViewModels
{
    public class PurchaseStatusViewModel
    {
        public required string OriginStationName { get; set; }
        public required string DestinationStationName { get; set; }
        public string TicketType { get; set; } = "Single Journey";
        public string PaymentMethod { get; set; } = "Account Balance";
        public DateTime ExpiryTime { get; set; }
        public string? TransactionId { get; set; }
        public decimal FareAmount { get; set; }
    }
}
