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
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        public StationDistanceRepository(ApplicationDbContext context, IMapper mapper) : base(context)
        {
            _db = context;
            _mapper = mapper;
        }

        public async Task<List<StationDistanceDto>> GetAllStationDistancesAsync()
        {
            var stationDistances = await _dbSet.ToListAsync();
            return _mapper.Map<List<StationDistanceDto>>(stationDistances);
        }
    }
}
