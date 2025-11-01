using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Repositories;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services;
using System.Text.Json;

namespace QRCodeBasedMetroTicketingSystem.Infrastructure.Services
{
    public class DistanceCalculationService : IDistanceCalculationService
    {
        private readonly ICacheService _cacheService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly string CacheKey = "Distance";

        public DistanceCalculationService(IUnitOfWork unitOfWork, ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
        }

        public async Task<decimal> GetDistanceBetweenAsync(int station1Id, int station2Id)
        {
            var cachedData = await _cacheService.GetAsync<string>(CacheKey);
            Dictionary<int, decimal>? cumulativeDistances = null;

            if (!string.IsNullOrWhiteSpace(cachedData))
            {
                cumulativeDistances = JsonSerializer.Deserialize<Dictionary<int, decimal>>(cachedData);
            }

            if (cumulativeDistances == null ||
                !cumulativeDistances.ContainsKey(station1Id) ||
                !cumulativeDistances.ContainsKey(station2Id))
            {
                cumulativeDistances = await CumulativeDistanceAsync();

                if (cumulativeDistances.Count > 0)
                {
                    string serializedData = JsonSerializer.Serialize(cumulativeDistances);
                    await _cacheService.SetAsync(CacheKey, serializedData, TimeSpan.FromHours(24));
                }
            }

            if (cumulativeDistances.ContainsKey(station1Id) && cumulativeDistances.ContainsKey(station2Id))
            {
                var distance1 = cumulativeDistances[station1Id];
                var distance2 = cumulativeDistances[station2Id];
                return Math.Abs(distance2 - distance1);
            }

            return -1;
        }

        public async Task<Dictionary<int, decimal>> CumulativeDistanceAsync()
        {
            var stationList = await _unitOfWork.StationRepository.GetAllStationsOrderedAsync();
            var stationDistances = await _unitOfWork.StationDistanceRepository.GetAllStationDistancesAsync();

            if (!stationDistances.Any())
            {
                return new Dictionary<int, decimal>();
            }

            Dictionary<int, decimal> cumulativeDistances = new();

            cumulativeDistances[stationList[0].Id] = 0;

            decimal cumulativeDistance = 0;
            for (int i = 0; i < stationList.Count - 1; i++)
            {
                int currentStationId = stationList[i].Id;
                int nextStationId = stationList[i + 1].Id;

                var distance = stationDistances.FirstOrDefault(d =>
                    (d.Station1Id == currentStationId && d.Station2Id == nextStationId) ||
                    (d.Station1Id == nextStationId && d.Station2Id == currentStationId)
                )?.Distance ?? 0;

                cumulativeDistance += distance;
                cumulativeDistances[nextStationId] = cumulativeDistance;
            }

            return cumulativeDistances;
        }
    }
}
