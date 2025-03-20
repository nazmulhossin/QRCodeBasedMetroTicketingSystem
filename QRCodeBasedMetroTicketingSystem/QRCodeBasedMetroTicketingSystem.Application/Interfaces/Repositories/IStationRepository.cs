using QRCodeBasedMetroTicketingSystem.Application.Common.Models.DataTables;
using QRCodeBasedMetroTicketingSystem.Application.DTOs;
using QRCodeBasedMetroTicketingSystem.Domain.Entities;

namespace QRCodeBasedMetroTicketingSystem.Application.Interfaces.Repositories
{
    public interface IStationRepository : IRepository<Station>
    {
        Task<DataTablesResponse<StationDto>> GetStationsDataTableAsync(DataTablesRequest request);
        Task<List<StationListDto>> GetStationsOrderedAsync();
        Task<Station?> GetStationByIdAsync(int stationId);
        Task<bool> StationExistsByNameAsync(string stationName, int? excludeStationId = null);
        Task UpdateSubsequentStationOrdersAsync(int fromOrder, int step);
        Task AddStationAsync(Station station);
        void DeleteStationAsync(Station station);
        Task DeleteDistanceBetweenAsync(int station1Id, int station2Id);
        Task AddStationDistanceAsync(int fromStation, int toStation, decimal distance);
        Task UpdateStationDistanceAsync(int fromStation, int toStation, decimal newDistance);
        Task<List<AdjacentStationDistanceDto>> GetAdjacentDistancesAsync(int stationId);
        Task<List<Station>> GetAllStationsOrderedAsync();
        Task SaveChangesAsync();
    }
}
