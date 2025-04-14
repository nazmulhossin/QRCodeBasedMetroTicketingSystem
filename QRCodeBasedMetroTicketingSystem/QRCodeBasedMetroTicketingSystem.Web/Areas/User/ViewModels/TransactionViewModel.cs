namespace QRCodeBasedMetroTicketingSystem.Web.Areas.User.ViewModels
{
    public class TransactionViewModel
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentFor { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
    }
}
