using QRCodeBasedMetroTicketingSystem.Application.DTOs;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Repositories;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services;

namespace QRCodeBasedMetroTicketingSystem.Infrastructure.Services
{
    public class FareCalculationService : IFareCalculationService
    {
        private readonly IDistanceCalculationService _distanceCalculationService;
        private readonly ISystemSettingsService _systemSettingsService;
        private readonly IStationRepository _stationRepository;

        public FareCalculationService(IDistanceCalculationService distanceCalculationService, ISystemSettingsService systemSettingsService, IStationRepository stationRepository)
        {
            _distanceCalculationService = distanceCalculationService;
            _systemSettingsService = systemSettingsService;
            _stationRepository = stationRepository;
        }

        public async Task<IEnumerable<FareDistanceDto>> GetFareDistancesAsync(int fromStationId, int? toStationId)
        {
            var systemSettings = await _systemSettingsService.GetCurrentSettingsAsync();
            var stationList = await _stationRepository.GetAllStationsOrderedAsync();

            // Result list to store fare and distance information
            var fareAndDistances = new List<FareDistanceDto>();

            var fromStation = stationList.FirstOrDefault(s => s.Id == fromStationId);
            if (fromStation == null)
                return fareAndDistances;

            if (toStationId.HasValue)
            {
                var toStation = stationList.FirstOrDefault(s => s.Id == toStationId.Value);
                if (toStation != null)
                {
                    var distance = await _distanceCalculationService.GetDistanceBetweenAsync(fromStationId, toStationId.Value);
                    var fare = CalculateFare(distance, systemSettings.FarePerKm, systemSettings.MinFare);

                    fareAndDistances.Add(new FareDistanceDto
                    {
                        FromStationName = fromStation.Name,
                        ToStationName = toStation.Name,
                        Distance = distance,
                        Fare = fare
                    });
                }
            }
            else
            {
                foreach (var toStation in stationList.Where(s => s.Id != fromStationId))
                {
                    var distance = await _distanceCalculationService.GetDistanceBetweenAsync(fromStationId, toStation.Id);
                    var fare = CalculateFare(distance, systemSettings.FarePerKm, systemSettings.MinFare);

                    fareAndDistances.Add(new FareDistanceDto
                    {
                        FromStationName = fromStation.Name,
                        ToStationName = toStation.Name,
                        Distance = distance,
                        Fare = fare
                    });
                }
            }

            return fareAndDistances;
        }

        private int CalculateFare(decimal distance, decimal farePerKm, decimal minFare)
        {
            decimal baseFare = distance * farePerKm;
            baseFare = Math.Max(baseFare, minFare);
            var roundToNearest = 10;
            int roundedFare = (int)Math.Round(baseFare / roundToNearest) * roundToNearest;

            return roundedFare;
        }
    }
}
