using AutoMapper;
using QRCodeBasedMetroTicketingSystem.Application.DTOs;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Repositories;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services;

namespace QRCodeBasedMetroTicketingSystem.Infrastructure.Services
{
    public class TripService : ITripService
    {
        private readonly ITripRepository _tripRepository;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public TripService(ITripRepository tripRepository, IUserService userService, IMapper mapper)
        {
            _tripRepository = tripRepository;
            _userService = userService;
            _mapper = mapper;
        }

        public async Task<List<TripDto>> GetCompletedTripsByUserIdAsync(int userId)
        {
            var trips = await _tripRepository.GetCompletedTripsByUserIdAsync(userId);
            var tripDtos = trips.Select(trip => _mapper.Map<TripDto>(trip)).ToList();

            return tripDtos;
        }
    }
}
