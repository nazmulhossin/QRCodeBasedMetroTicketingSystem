using QRCodeBasedMetroTicketingSystem.Application.DTOs;

namespace QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services
{
    public interface IFareCalculationService
    {
        Task<IEnumerable<FareDistanceDto>> GetFareDistancesAsync(int fromStationId, int? toStationId);
    }
}
