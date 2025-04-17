using QRCodeBasedMetroTicketingSystem.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace QRCodeBasedMetroTicketingSystem.Web.Areas.User.ViewModels
{
    public class AddBalanceViewModel
    {
        [Required]
        [Range(10, 10000, ErrorMessage = "Amount must be between 10 and 10,000")]
        public decimal Amount { get; set; }

        [Required]
        public PaymentMethod PaymentMethod { get; set; }
    }
}
