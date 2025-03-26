using QRCodeBasedMetroTicketingSystem.Application.Common.Models.DataTables;
using QRCodeBasedMetroTicketingSystem.Application.DTOs;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Repositories;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services;

namespace QRCodeBasedMetroTicketingSystem.Infrastructure.Services
{
    public class FareAndDistanceService : IFareAndDistanceService
    {
        public readonly IStationRepository _stationRepository;
        private readonly IFareCalculationService _fareCalculationService;

        public FareAndDistanceService(IStationRepository stationRepository, IFareCalculationService fareCalculationService)
        {
            _stationRepository = stationRepository;
            _fareCalculationService = fareCalculationService;
        }

        public async Task<FareAndDistancesDto> GetFareAndDistanceModel()
        {
            var stationList = await _stationRepository.GetAllStationsOrderedAsync();
            return new FareAndDistancesDto { StationList = stationList };
        }

        public async Task<DataTablesResponse<FareDistanceDto>> GetFareDistanceDataTablesAsync(DataTablesRequest request, int? fromStationId, int? toStationId)
        {
            if (!fromStationId.HasValue)
            {
                return new DataTablesResponse<FareDistanceDto>
                {
                    Draw = request.Draw,
                    RecordsTotal = 0,
                    RecordsFiltered = 0,
                    Data = new List<FareDistanceDto>()
                };
            }

            var fareDistances = await _fareCalculationService.GetFareDistancesAsync(fromStationId.Value, toStationId);

            // Apply search if needed
            if (!string.IsNullOrEmpty(request.Search?.Value))
            {
                fareDistances = fareDistances.Where(fd =>
                    fd.FromStationName!.Contains(request.Search.Value, StringComparison.OrdinalIgnoreCase) ||
                    fd.ToStationName!.Contains(request.Search.Value, StringComparison.OrdinalIgnoreCase)
                );
            }

            // Apply sorting
            if (request.Order != null && request.Order.Count > 0)
            {
                var orderColumn = request.Columns![request.Order[0].Column].Data;
                var orderDir = request.Order[0].Dir;

                fareDistances = orderColumn switch
                {
                    "fromStationName" => orderDir == "asc"
                        ? fareDistances.OrderBy(fd => fd.FromStationName)
                        : fareDistances.OrderByDescending(fd => fd.FromStationName),
                    "toStationName" => orderDir == "asc"
                        ? fareDistances.OrderBy(fd => fd.ToStationName)
                        : fareDistances.OrderByDescending(fd => fd.ToStationName),
                    "distance" => orderDir == "asc"
                        ? fareDistances.OrderBy(fd => fd.Distance)
                        : fareDistances.OrderByDescending(fd => fd.Distance),
                    _ => fareDistances
                };
            }

            // Calculate total count
            var totalCount = fareDistances.Count();

            // Apply pagination
            var paginatedData = fareDistances
                .Skip(request.Start)
                .Take(request.Length)
                .ToList();

            return new DataTablesResponse<FareDistanceDto>
            {
                Draw = request.Draw,
                RecordsTotal = totalCount,
                RecordsFiltered = totalCount,
                Data = paginatedData
            };
        }
    }
}
