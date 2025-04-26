using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services;
using System.Globalization;
using System.Runtime.InteropServices;

namespace QRCodeBasedMetroTicketingSystem.Infrastructure.Services
{
    public class TimeService : ITimeService
    {
        public DateTime ConvertUtcToBdTime(DateTime utcDateTime)
        {
            if (utcDateTime.Kind != DateTimeKind.Utc)
            {
                utcDateTime = DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc);
            }

            var timeZoneId = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? "Bangladesh Standard Time"
                : "Asia/Dhaka";

            try
            {
                var bdTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
                return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, bdTimeZone);
            }
            catch (TimeZoneNotFoundException)
            {
                return utcDateTime.AddHours(6);
            }
        }

        public string FormatAsBdTime(DateTime utcTime, string? format = null)
        {
            DateTime bdTime = ConvertUtcToBdTime(utcTime);

            if (string.IsNullOrEmpty(format))
            {
                return bdTime.ToString("dd MMM yyyy, h:mm tt", CultureInfo.InvariantCulture);
            }

            return bdTime.ToString(format, CultureInfo.InvariantCulture);
        }
    }
}
