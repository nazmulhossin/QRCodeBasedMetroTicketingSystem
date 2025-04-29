using QRCodeBasedMetroTicketingSystem.Application.DTOs;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Repositories;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services;
using System.Globalization;

namespace QRCodeBasedMetroTicketingSystem.Infrastructure.Services
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AdminDashboardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<AdminDashboardDto> GetDashboardDataAsync()
        {
            // Get total registered users
            var totalRegisteredUsers = await _unitOfWork.UserRepository.GetTotalRegisteredUsersAsync();

            // Get current passenger count
            var currentPassengerCount = await _unitOfWork.TripRepository.GetCurrentPassengerCountAsync();

            // Get passenger traffic data for the last 30 days
            var passengerData = await _unitOfWork.TripRepository.GetDailyPassengerCountAsync(30);

            // Get revenue data for the last 12 months
            var revenueData = await _unitOfWork.TransactionRepository.GetMonthlyRevenueAsync(12);

            // Get ticket type distribution data
            var ticketTypeData = await _unitOfWork.TicketRepository.GetTicketTypeDistributionByMonthAsync(6);

            // Get summary data
            int totalPassengers = passengerData.Values.Sum();
            decimal totalRevenue = await _unitOfWork.TransactionRepository.GetTotalRevenueAsync();

            // Convert to view model
            return new AdminDashboardDto
            {
                TotalRegisteredUsers = totalRegisteredUsers,
                CurrentPassengerCount = currentPassengerCount,
                TotalRevenue = totalRevenue,
                QRTickets = ticketTypeData.ContainsKey("QRTicket") ? ticketTypeData["QRTicket"] : 0,
                RapidPasses = ticketTypeData.ContainsKey("RapidPass") ? ticketTypeData["RapidPass"] : 0,
                PassengerTraffic = passengerData.Select(kv => new ChartDataPoint
                {
                    Label = kv.Key.ToString("MMM dd"),
                    Value = kv.Value
                }).ToList(),
                RevenueAnalysis = revenueData.Select(kv => new ChartDataPoint
                {
                    Label = DateTime.ParseExact(kv.Key, "yyyy-MM", CultureInfo.InvariantCulture).ToString("MMM yyyy"),
                    Value = (double)kv.Value
                }).ToList()
            };
        }
    }
}
