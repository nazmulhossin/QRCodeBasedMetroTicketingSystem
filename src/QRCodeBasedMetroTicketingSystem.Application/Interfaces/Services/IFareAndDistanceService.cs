using QRCodeBasedMetroTicketingSystem.Application.Common.Models.DataTables;
using QRCodeBasedMetroTicketingSystem.Application.DTOs;

namespace QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services
{
    public interface IFareAndDistanceService
    {
        Task<FareAndDistancesDto> GetFareAndDistanceModel();
        Task<DataTablesResponse<FareDistanceDto>> GetFareDistanceDataTablesAsync(DataTablesRequest request, int? fromStationId = null, int? toStationId = null);
    }
}
