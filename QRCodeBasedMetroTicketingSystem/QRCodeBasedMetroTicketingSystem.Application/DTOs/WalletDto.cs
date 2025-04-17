namespace QRCodeBasedMetroTicketingSystem.Application.DTOs
{
    public class WalletDto
    {
        public int Id { get; set; }
        public decimal Balance { get; set; }
        public List<TransactionDto> RecentTransactions { get; set; }
    }
}
