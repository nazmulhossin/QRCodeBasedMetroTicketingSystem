using System.ComponentModel.DataAnnotations;

namespace QRCodeBasedMetroTicketingSystem.Domain.Entities
{
    public class Admin
    {
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(255)]
        public required string PasswordHash { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginAt { get; set; }
    }
}
