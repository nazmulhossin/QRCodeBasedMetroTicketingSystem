namespace QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services
{
    public interface IDistanceCalculationService
    {
        Task<decimal> GetDistanceBetweenAsync(int Station1Id, int Station2Id);
    }
}
