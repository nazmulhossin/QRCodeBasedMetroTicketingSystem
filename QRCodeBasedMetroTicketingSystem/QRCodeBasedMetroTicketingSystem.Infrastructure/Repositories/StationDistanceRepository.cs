using AutoMapper;
using Microsoft.EntityFrameworkCore;
using QRCodeBasedMetroTicketingSystem.Application.DTOs;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Repositories;
using QRCodeBasedMetroTicketingSystem.Domain.Entities;
using QRCodeBasedMetroTicketingSystem.Infrastructure.Data;

namespace QRCodeBasedMetroTicketingSystem.Infrastructure.Repositories
{
    public class StationDistanceRepository : Repository<StationDistance>, IStationDistanceRepository
    {
        private readonly IMapper _mapper;
        public StationDistanceRepository(ApplicationDbContext context, IMapper mapper) : base(context)
        {
            _mapper = mapper;
        }

        public async Task<List<StationDistanceDto>> GetAllStationDistancesAsync()
        {
            var stationDistances = await _dbSet.ToListAsync();
            return _mapper.Map<List<StationDistanceDto>>(stationDistances);
        }

        public async Task DeleteDistanceBetweenAsync(int station1Id, int station2Id)
        {
            var distance = await _dbSet
                .Where(d => (d.Station1Id == station1Id && d.Station2Id == station2Id) ||
                            (d.Station1Id == station2Id && d.Station2Id == station1Id))
                .ToListAsync();

            _dbSet.RemoveRange(distance);
        }

        public async Task AddStationDistanceAsync(int fromStation, int toStation, decimal distance)
        {
            var stationDistance = new StationDistance
            {
                Station1Id = fromStation,
                Station2Id = toStation,
                Distance = distance
            };

            await _dbSet.AddAsync(stationDistance);
        }

        public async Task UpdateStationDistanceAsync(int fromStation, int toStation, decimal newDistance)
        {
            await _dbSet
                .Where(s =>
                    (s.Station1Id == fromStation && s.Station2Id == toStation) ||
                    (s.Station1Id == toStation && s.Station2Id == fromStation))
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(s => s.Distance, s => newDistance));
        }

        public async Task<List<AdjacentStationDistanceDto>> GetAdjacentDistancesAsync(int stationId)
        {
            return await _dbSet
                .Where(d => d.Station1Id == stationId || d.Station2Id == stationId)
                .Select(d => new AdjacentStationDistanceDto
                {
                    StationId = stationId,
                    AdjacentStationId = d.Station1Id == stationId ? d.Station2Id : d.Station1Id,
                    AdjacentStationName = d.Station1Id == stationId ? d.Station2!.Name : d.Station1!.Name,
                    Distance = d.Distance
                })
                .ToListAsync();
        }
    }
}
