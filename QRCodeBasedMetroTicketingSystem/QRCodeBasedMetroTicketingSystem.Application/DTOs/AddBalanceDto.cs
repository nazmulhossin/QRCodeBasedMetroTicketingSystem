using QRCodeBasedMetroTicketingSystem.Domain.Entities;

namespace QRCodeBasedMetroTicketingSystem.Application.DTOs
{
    public class AddBalanceDto
    {
        public required decimal Amount { get; set; }
        public required PaymentMethod PaymentMethod { get; set; }
    }
}
