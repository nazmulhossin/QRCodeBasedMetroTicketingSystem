using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services;
using QRCodeBasedMetroTicketingSystem.Domain.Entities;
using QRCoder;
using System.Security.Cryptography;
using System.Text;

namespace QRCodeBasedMetroTicketingSystem.Infrastructure.Services
{
    public class QRCodeService : IQRCodeService
    {
        public string GenerateQRCodeData(Ticket ticket)
        {
            // Create a unique, secure QR code data
            var timestamp = DateTime.UtcNow.Ticks;
            var expiryTimestamp = ticket.ExpiryTime.Ticks;

            var dataToHash = $"{ticket.Id}|{ticket.UserId}|{(int)ticket.Type}|{expiryTimestamp}|{timestamp}";

            using (var sha256 = SHA256.Create())
            {
                var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(dataToHash));
                var hash = BitConverter.ToString(hashBytes).Replace("-", "").Substring(0, 16);

                // Format: TicketId|UserId|TicketType|ExpiryTimestamp|Hash|Timestamp
                return $"{ticket.Id}|{ticket.UserId}|{(int)ticket.Type}|{expiryTimestamp}|{hash}|{timestamp}";
            }
        }

        public string GenerateQRCode(string qrCodeData)
        {
            using QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeDataObj = qrGenerator.CreateQrCode(qrCodeData, QRCodeGenerator.ECCLevel.Q);

            using BitmapByteQRCode qrCode = new BitmapByteQRCode(qrCodeDataObj);
            byte[] qrCodeBytes = qrCode.GetGraphic(20);
            string qrCodeBase64 = Convert.ToBase64String(qrCodeBytes);

            return qrCodeBase64;
        }
    }
}
