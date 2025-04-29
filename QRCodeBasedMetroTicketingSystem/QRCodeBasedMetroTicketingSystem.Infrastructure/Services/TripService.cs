using AutoMapper;
using QRCodeBasedMetroTicketingSystem.Application.DTOs;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Repositories;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services;

namespace QRCodeBasedMetroTicketingSystem.Infrastructure.Services
{
    public class TripService : ITripService
    {
        private readonly ITripRepository _tripRepository;
        private readonly IMapper _mapper;

        public TripService(ITripRepository tripRepository, IUserService userService, IMapper mapper)
        {
            _tripRepository = tripRepository;
            _mapper = mapper;
        }

        public async Task<List<TripDto>> GetCompletedTripsByUserIdAsync(int userId)
        {
            var thirtyDaysAgoDate = DateTime.UtcNow.AddDays(-30);
            var trips = await _tripRepository.GetCompletedTripsByUserIdAsync(userId, thirtyDaysAgoDate);
            var tripDtos = trips.Select(trip => _mapper.Map<TripDto>(trip)).ToList();

            return tripDtos;
        }
    }
}
