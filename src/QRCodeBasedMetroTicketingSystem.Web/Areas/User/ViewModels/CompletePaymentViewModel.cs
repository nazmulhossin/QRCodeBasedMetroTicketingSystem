using QRCodeBasedMetroTicketingSystem.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace QRCodeBasedMetroTicketingSystem.Web.Areas.User.ViewModels
{
    public class CompletePaymentViewModel
    {
        [Required]
        public string TransactionReference { get; set; }

        [Required]
        public PaymentMethod PaymentMethod { get; set; }
    }
}
