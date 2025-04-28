using QRCodeBasedMetroTicketingSystem.Application.DTOs;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Repositories;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services;
using QRCodeBasedMetroTicketingSystem.Domain.Entities;

namespace QRCodeBasedMetroTicketingSystem.Infrastructure.Services
{
    public class UserDashboardService : IUserDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITicketService _ticketService;

        public UserDashboardService(IUnitOfWork unitOfWork, ITicketService ticketService)
        {
            _unitOfWork = unitOfWork;
            _ticketService = ticketService;
        }

        public async Task<UserDashboardStatsDto> GetUserStatsAsync(int userId)
        {
            // Get the first day of the current month
            var startOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);

            var totalTopUps = await _unitOfWork.TransactionRepository.GetTotalTopUpsAsync(userId, startOfMonth);
            var totalTrips = await _unitOfWork.TripRepository.GetTripCountByUserIdAsync(userId, startOfMonth);
            var validTickets = await _ticketService.GetValidQrTicketsCountByUserIdAsync(userId);

            return new UserDashboardStatsDto
            {
                TotalTopUps = totalTopUps,
                TotalTrips = totalTrips,
                ValidTickets = validTickets
            };
        }

        public async Task<List<UserActivityDto>> GetRecentActivitiesAsync(int userId, int count = 10)
        {
            var activities = new List<UserActivityDto>();

            // Get recent transactions
            var transactions = await _unitOfWork.TransactionRepository.GetRecentTransactionsByUserIdAsync(userId);
            foreach (var transaction in transactions)
            {
                ActivityType activityType;
                bool isCredit = false;

                switch (transaction.Type)
                {
                    case TransactionType.TopUp:
                        activityType = ActivityType.TopUp;
                        isCredit = true;
                        break;
                    case TransactionType.Payment:
                        if (transaction.PaymentFor == PaymentItem.QRTicket && transaction.PaymentMethod != PaymentMethod.AccountBalance)
                            activityType = ActivityType.OnlinePayment;
                        else
                            activityType = ActivityType.AccountBalancePayment;
                        break;
                    default:
                        activityType = ActivityType.AccountBalancePayment;
                        break;
                }

                activities.Add(new UserActivityDto
                {
                    Type = activityType,
                    Description = transaction.Description,
                    Time = transaction.CreatedAt,
                    Amount = transaction.Amount,
                    IsCredit = isCredit
                });
            }

            // Get recent trips
            var trips = await _unitOfWork.TripRepository.GetRecentTripsByUserIdAsync(userId);
            foreach (var trip in trips)
            {
                string description;
                if (trip.ExitStationId.HasValue)
                {
                    description = $"Completed trip from {trip.EntryStation.Name} to {trip.ExitStation.Name}";
                    if (trip.Ticket.Type == TicketType.RapidPass)
                        description += " using Rapid Pass";
                    else
                        description += " using QR Ticket";
                }
                else
                {
                    description = $"Started trip from {trip.EntryStation.Name}";
                }

                activities.Add(new UserActivityDto
                {
                    Type = ActivityType.Trip,
                    Description = description,
                    Time = trip.EntryTime,
                    Amount = null,
                    IsCredit = false
                });
            }

            // Sort by time and take the requested count
            return activities
                .OrderByDescending(a => a.Time)
                .Take(count)
                .ToList();
        }
    }
}
