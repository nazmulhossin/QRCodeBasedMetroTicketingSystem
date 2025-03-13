using QRCodeBasedMetroTicketingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QRCodeBasedMetroTicketingSystem.Application.Interfaces
{
    public interface IStationDistanceRepository
    {
        Task<List<DistanceViewModel>> GetAdjacentDistancesAsync(int stationId);
        Task AddDistanceAsync(StationDistance distance);
        Task RemoveDistancesAsync(int stationId);
        Task RemoveSpecificDistancesAsync(int station1Id, int station2Id);
        Task CreateDistanceBetweenAdjacentStationsAsync(int station1Id, int station2Id, decimal distance);
    }
}
