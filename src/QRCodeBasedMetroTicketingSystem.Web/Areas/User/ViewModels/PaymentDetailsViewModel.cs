using QRCodeBasedMetroTicketingSystem.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace QRCodeBasedMetroTicketingSystem.Web.Areas.User.ViewModels
{
    public class PaymentDetailsViewModel
    {
        [Required]
        [Range(20, 10000, ErrorMessage = "Amount must be between 20 and 10,000")]
        public decimal Amount { get; set; }

        [Required]
        public PaymentMethod PaymentMethod { get; set; }

        public string? TransactionReference { get; set; }
    }
}
