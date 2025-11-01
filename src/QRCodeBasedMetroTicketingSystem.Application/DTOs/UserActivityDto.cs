namespace QRCodeBasedMetroTicketingSystem.Application.DTOs
{
    public class UserActivityDto
    {
        public ActivityType Type { get; set; }
        public string Description { get; set; }
        public DateTime Time { get; set; }
        public decimal? Amount { get; set; }
        public bool IsCredit { get; set; }
    }

    public enum ActivityType
    {
        TopUp,
        OnlinePayment, 
        AccountBalancePayment,
        Trip,
        Refund
    }
}
