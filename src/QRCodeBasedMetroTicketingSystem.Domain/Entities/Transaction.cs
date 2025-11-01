using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace QRCodeBasedMetroTicketingSystem.Domain.Entities
{
    public class Transaction
    {
        public int Id { get; set; }

        [Required]
        public int WalletId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        public TransactionType Type { get; set; }

        [Required]
        public PaymentMethod PaymentMethod { get; set; }

        public PaymentItem? PaymentFor { get; set; }

        [Required]
        public TransactionStatus Status { get; set; }

        [StringLength(50)]
        public string? TransactionReference { get; set; }

        [StringLength(255)]
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        [ForeignKey("WalletId")]
        public Wallet Wallet { get; set; }
    }

    public enum TransactionType
    {
        TopUp,
        Payment,
        Refund,
        Penalty
    }

    public enum PaymentMethod
    {
        BKash,
        Rocket,
        Nagad,
        VisaCard,
        MasterCard,
        AccountBalance,
        System
    }

    public enum PaymentItem
    {
        None,
        QRTicket,
        RapidPass
    }

    public enum TransactionStatus
    {
        Pending,
        Completed,
        Failed,
        Canceled,
        Refunded
    }
}
