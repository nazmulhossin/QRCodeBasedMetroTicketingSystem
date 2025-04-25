using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services;
using System.Globalization;

namespace QRCodeBasedMetroTicketingSystem.Infrastructure.Services
{
    public class TimeService : ITimeService
    {
        private static readonly TimeZoneInfo BdTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Dhaka");

        public DateTime ConvertUtcToBdTime(DateTime utcTime)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(utcTime, BdTimeZone);
        }

        public string FormatAsBdTime(DateTime utcTime, string format = null)
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
