namespace QRCodeBasedMetroTicketingSystem.Web.Areas.User.ViewModels
{
    public class WalletViewModel
    {
        public decimal Balance { get; set; }
        public List<TransactionViewModel> RecentTransactions { get; set; }
    }
}
