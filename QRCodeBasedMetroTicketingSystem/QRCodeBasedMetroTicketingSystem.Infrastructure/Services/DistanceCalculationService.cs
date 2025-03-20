using QRCodeBasedMetroTicketingSystem.Application.DTOs;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Repositories;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services;
using System.Text.Json;
namespace QRCodeBasedMetroTicketingSystem.Infrastructure.Services
{
    public class DistanceCalculationService : IDistanceCalculationService
    {
        private readonly IStationRepository _stationRepository;
        private readonly IStationDistanceRepository _stationDistanceRepository;
        private readonly ICacheService _cacheService;
        private readonly string CacheKey = "Distance";

        public DistanceCalculationService(IStationRepository stationRepository, IStationDistanceRepository stationDistanceRepository, ICacheService cacheService)
        {
            _stationRepository = stationRepository;
            _stationDistanceRepository = stationDistanceRepository;
            _cacheService = cacheService;
        }

        public async Task<List<StationListDto>> GetStationAsync()
        {
            var stations = await _stationRepository.GetAllStationsOrderedAsync();

            var StationListDtos = stations.Select(station => new StationListDto
            {
                StationId = station.Id,
                StationName = station.Name,
                Order = station.Order
            }).ToList();

            return StationListDtos;
        }

        public async Task<List<StationDistanceDto>> GetStationDistanceAsync()
        {
            var stationDistances = await _stationDistanceRepository.GetAllStationDistancesAsync();

            var StationDistanceDtos = stationDistances.Select(distance => new StationDistanceDto
            {
                Station1Id = distance.Station1Id,
                Station2Id = distance.Station2Id,
                Distance = distance.Distance
            }).ToList();

            return StationDistanceDtos;
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

            Console.WriteLine($"Unable to calculate distance between station {station1Id} and station {station2Id}");
            return -1;
        }

        public async Task<Dictionary<int, decimal>> CumulativeDistanceAsync()
        {
            var stations = await GetStationAsync();
            var distances = await GetStationDistanceAsync();
            if (!stations.Any() || !distances.Any())
            {
                return new Dictionary<int, decimal>();
            }

            Dictionary<int, decimal> cumulativeDistances = new();

            cumulativeDistances[stations[0].StationId] = 0;

            decimal cumulativeDistance = 0;
            for (int i = 0; i < stations.Count - 1; i++)
            {
                int currentStationId = stations[i].StationId;
                int nextStationId = stations[i + 1].StationId;

                var distance = distances.FirstOrDefault(d =>
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
