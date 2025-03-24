using QRCodeBasedMetroTicketingSystem.Application.Common.Models.DataTables;
using QRCodeBasedMetroTicketingSystem.Application.DTOs;
using QRCodeBasedMetroTicketingSystem.Domain.Entities;

namespace QRCodeBasedMetroTicketingSystem.Application.Interfaces.Repositories
{
    public interface IStationRepository : IRepository<Station>
    {
        Task<DataTablesResponse<StationDto>> GetStationsDataTableAsync(DataTablesRequest request);
        Task<List<StationListDto>> GetAllStationsOrderedAsync();
        Task<Station?> GetStationByIdAsync(int stationId);
        Task<bool> StationExistsByNameAsync(string stationName, int? excludeStationId = null);
        Task UpdateSubsequentStationOrdersAsync(int fromOrder, int step);
        Task AddStationAsync(Station station);
        Task DeleteStationAsync(Station station);
    }
}
