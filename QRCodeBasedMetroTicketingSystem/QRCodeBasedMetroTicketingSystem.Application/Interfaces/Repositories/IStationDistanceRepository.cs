using QRCodeBasedMetroTicketingSystem.Application.DTOs;
using QRCodeBasedMetroTicketingSystem.Domain.Entities;

namespace QRCodeBasedMetroTicketingSystem.Application.Interfaces.Repositories
{
    public interface IStationDistanceRepository : IRepository<StationDistance>
    {
        Task<List<StationDistanceDto>> GetAllStationDistancesAsync();
        Task DeleteDistanceBetweenAsync(int station1Id, int station2Id);
        Task AddStationDistanceAsync(int fromStation, int toStation, decimal distance);
        Task UpdateStationDistanceAsync(int fromStation, int toStation, decimal newDistance);
        Task<List<AdjacentStationDistanceDto>> GetAdjacentDistancesAsync(int stationId);
    }
}