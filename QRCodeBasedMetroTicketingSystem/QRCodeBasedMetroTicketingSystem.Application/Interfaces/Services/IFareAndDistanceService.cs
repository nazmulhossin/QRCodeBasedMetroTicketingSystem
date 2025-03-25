using QRCodeBasedMetroTicketingSystem.Application.DTOs;

namespace QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services
{
    public interface IFareAndDistanceService
    {
        Task<FareAndDistancesDto> GetFareAndDistanceModel();
    }
}
