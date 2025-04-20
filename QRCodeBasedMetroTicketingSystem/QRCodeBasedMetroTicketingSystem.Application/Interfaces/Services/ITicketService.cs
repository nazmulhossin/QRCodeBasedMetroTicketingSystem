namespace QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services
{
    public interface ITicketService
    {
        Task<(string OriginStationName, string DestinationStationName, int Fare)> GetTicketSummaryAsync(int fromStationId, int toStationId);
    }
}
