using QRCodeBasedMetroTicketingSystem.Application.DTOs;
using QRCodeBasedMetroTicketingSystem.Domain.Entities;

namespace QRCodeBasedMetroTicketingSystem.Application.Interfaces.Repositories
{
    public interface IStationDistanceRepository : IRepository<StationDistance>
    {
        Task<List<StationDistanceDto>> GetAllStationDistancesAsync();
    }
}