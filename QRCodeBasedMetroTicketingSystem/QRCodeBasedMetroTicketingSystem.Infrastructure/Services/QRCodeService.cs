using Microsoft.Extensions.Configuration;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services;
using QRCodeBasedMetroTicketingSystem.Domain.Entities;
using QRCoder;
using System.Security.Cryptography;
using System.Text;

namespace QRCodeBasedMetroTicketingSystem.Infrastructure.Services
{
    public class QRCodeService : IQRCodeService
    {
        private readonly IConfiguration _configuration;
        private readonly int hashLength = 16;
        private readonly int noOfPartsInQRCode = 4;

        public QRCodeService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateQRCodeData(Ticket ticket)
        {
            // Create a unique, secure QR code data
            var timestamp = DateTime.UtcNow.Ticks;
            var expiryTimestamp = ticket.ExpiryTime.Ticks;
            var secretKey = _configuration["QRCodeSecretKey"];
            var dataToHash = $"{ticket.Id}|{secretKey}|{expiryTimestamp}|{timestamp}";

            using (var sha256 = SHA256.Create())
            {
                var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(dataToHash));
                var hash = BitConverter.ToString(hashBytes).Replace("-", "").Substring(0, hashLength);

                // Format: TicketId|ExpiryTimestamp|Hash|Timestamp
                return $"{ticket.Id}|{expiryTimestamp}|{hash}|{timestamp}";
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

        public bool ValidateQRCodeData(string qrCodeData)
        {
            // Check format: TicketId|ExpiryTimestamp|Hash|Timestamp
            var parts = qrCodeData.Split('|');
            if (parts.Length != noOfPartsInQRCode)
                return false;

            // Validate each part
            if (!int.TryParse(parts[0], out _))  // TicketId
                return false;
            if (!long.TryParse(parts[1], out _)) // ExpiryTimestamp
                return false;
            if (parts[2].Length != hashLength)   // Hash
                return false;
            if (!long.TryParse(parts[3], out _)) // Timestamp
                return false;

            return true;
        }

        public int ParseQRCodeDataToGetTicketId(string qrCodeData)
        {
            var parts = qrCodeData.Split('|');
            return int.Parse(parts[0]);
        }
    }
}
