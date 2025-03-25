using QRCodeBasedMetroTicketingSystem.Application.DTOs;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Repositories;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services;

namespace QRCodeBasedMetroTicketingSystem.Infrastructure.Services
{
    public class FareAndDistanceService : IFareAndDistanceService
    {
        public readonly IStationRepository _stationRepository;

        public FareAndDistanceService(IStationRepository stationRepository)
        {
            _stationRepository = stationRepository;
        }

        public async Task<FareAndDistancesDto> GetFareAndDistanceModel()
        {
            var stationList = await _stationRepository.GetAllStationsOrderedAsync();
            return new FareAndDistancesDto { StationList = stationList };
        }
    }
}
