namespace QRCodeBasedMetroTicketingSystem.Application.DTOs
{
    public class TransactionDto
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; }
        public string PaymentMethod { get; set; }
        public string Status { get; set; }
        public string TransactionReference { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
