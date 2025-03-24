using QRCodeBasedMetroTicketingSystem.Application.Common.Models.DataTables;
using QRCodeBasedMetroTicketingSystem.Application.Common.Result;
using QRCodeBasedMetroTicketingSystem.Application.DTOs;

namespace QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services
{
    public interface IStationService
    {
        Task<DataTablesResponse<StationDto>> GetStationsDataTableAsync(DataTablesRequest request);
        Task<bool> StationExistsByNameAsync(string stationName, int? excludeStationId = null);
        Task<StationCreationDto> GetStationCreationModelAsync();
        Task<Result> CreateStationAsync(StationCreationDto model);
        Task<StationEditDto?> GetStationEditModelAsync(int StationId);
        Task<Result> UpdateStationAsync(StationEditDto model);
        Task<StationDeletionDto?> GetStationDeletionModelAsync(int StationId);
        Task<Result> DeleteStationAsync(int StationId);
    }
}
