namespace QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services
{
    public interface ITimeService
    {
        DateTime ConvertUtcToBdTime(DateTime utcTime);
        string FormatAsBdTime(DateTime utcTime, string format = null);
    }
}
