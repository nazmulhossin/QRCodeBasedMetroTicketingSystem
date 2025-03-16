using QRCodeBasedMetroTicketingSystem.Application.Common.Models.DataTables;
using QRCodeBasedMetroTicketingSystem.Application.DTOs;
using QRCodeBasedMetroTicketingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace QRCodeBasedMetroTicketingSystem.Application.Interfaces.Repositories
{
    public interface IStationRepository : IRepository<Station>
    {
        Task<DataTablesResponse<StationDto>> GetStationsDataTableAsync(DataTablesRequest request);
        Task<List<StationListDto>> GetStationsOrderedAsync();
        Task<Station> GetStationByIdAsync(int stationId);
        Task<bool> StationExistsByNameAsync(string stationName);
        Task UpdateSubsequentStationOrdersAsync(int fromOrder);
        Task AddStationAsync(Station station);
        Task DeleteExistingDistancesAsync(int station1Id, int station2Id);
        Task AddStationDistancesAsync(int stationId, Dictionary<int, decimal> distances);
        Task SaveChangesAsync();
    }
}
