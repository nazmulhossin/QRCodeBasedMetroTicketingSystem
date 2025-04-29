using QRCodeBasedMetroTicketingSystem.Application.DTOs;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Repositories;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services;
using QRCodeBasedMetroTicketingSystem.Domain.Entities;

namespace QRCodeBasedMetroTicketingSystem.Infrastructure.Services
{
    public class TicketScanService : ITicketScanService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWalletService _walletService;
        private readonly IPaymentService _paymentService;
        private readonly IQRCodeService _qrCodeService;
        private readonly ITimeService _timeService;
        private readonly IFareCalculationService _fareCalculationService;
        private readonly ISystemSettingsService _systemSettingsService;
        private readonly string successMessage = "GO! GO!";

        public TicketScanService(
            IUnitOfWork unitOfWork,
            IWalletService walletService,
            IPaymentService paymentService,
            IQRCodeService qrCodeService,
            ITimeService timeService,
            IFareCalculationService fareCalculationService,
            ISystemSettingsService systemSettingsService)
        {
            _unitOfWork = unitOfWork;
            _walletService = walletService;
            _paymentService = paymentService;
            _qrCodeService = qrCodeService;
            _timeService = timeService;
            _fareCalculationService = fareCalculationService;
            _systemSettingsService = systemSettingsService;
        }

        public async Task<ScanTicketResponseDto> ProcessTicketScanAsync(ScanTicketRequest request)
        {
            var defaultInvalidResponse = new ScanTicketResponseDto { IsValid = false, Message = "Invalid" };

            if (!_qrCodeService.ValidateQRCodeData(request.QRCodeData))
            {
                return defaultInvalidResponse;
            }

            var ticketId = _qrCodeService.ParseQRCodeDataToGetTicketId(request.QRCodeData);
            var ticket = await _unitOfWork.TicketRepository.GetTicketByIdAsync(ticketId);

            if (ticket == null)
            {
                return defaultInvalidResponse;
            }

            // Validate QRCodeData with server data
            if (ticket.Status == TicketStatus.Active && ticket.ExpiryTime < DateTime.UtcNow)
            {
                return new ScanTicketResponseDto { IsValid = false, Message = "Ticket has expired" };
            }
            if (ticket.Status == TicketStatus.Used)
            {
                return new ScanTicketResponseDto { IsValid = false, Message = "Ticket already used" };
            }
            if (ticket.QRCodeData != request.QRCodeData)
            {
                return defaultInvalidResponse;
            }

            // Check if the station exists or not
            var station = await _unitOfWork.StationRepository.GetStationByIdAsync(request.StationId);
            if (station == null)
            {
                return defaultInvalidResponse;
            }

            // Get system settings for validations
            var systemSettings = await _systemSettingsService.GetSystemSettingsAsync();
            if (systemSettings == null)
            {
                return new ScanTicketResponseDto { IsValid = false, Message = "Server Error!" };
            }

            // Handle both ticket types
            if (ticket.Type == TicketType.QRTicket)
            {
                return await HandleQRTicketScanAsync(ticket, station, systemSettings, defaultInvalidResponse);
            }
            else // RapidPass
            {
                return await HandleRapidPassScanAsync(ticket, station, systemSettings, defaultInvalidResponse);
            }
        }

        private async Task<ScanTicketResponseDto> HandleQRTicketScanAsync(Ticket ticket, Station station, SystemSettingsDto systemSettings, ScanTicketResponseDto defaultInvalidResponse)
        {
            // If the ticket is Active, this is an entry scan
            if (ticket.Status == TicketStatus.Active)
            {
                // Check if the ticket's origin station matches the scanned station
                if (ticket.OriginStationId != station.Id)
                {
                    return defaultInvalidResponse;
                }

                // Create new trip
                var trip = new Trip
                {
                    UserId = ticket.UserId,
                    TicketId = ticket.Id,
                    EntryStationId = station.Id,
                    EntryTime = DateTime.UtcNow,
                    Status = TripStatus.InProgress
                };

                await _unitOfWork.TripRepository.CreateTripAsync(trip); // Save trip
                ticket.Status = TicketStatus.InUse;   // Update ticket status
                await _unitOfWork.SaveChangesAsync(); // Save changes

                return new ScanTicketResponseDto { IsValid = true, Message = successMessage };
            }
            // If the ticket is InUse, this is an exit scan
            else if (ticket.Status == TicketStatus.InUse)
            {
                // Check if the ticket's destination station matches the scanned station
                if (ticket.DestinationStationId != station.Id)
                {
                    return defaultInvalidResponse;
                }

                // Find active(in-progress) trip
                var trip = await _unitOfWork.TripRepository.GetActiveTripByTicketIdAsync(ticket.Id);
                if (trip == null)
                {
                    return defaultInvalidResponse;
                }

                // Check time limit
                var tripDuration = DateTime.UtcNow - trip.EntryTime;
                if (tripDuration.TotalMinutes > systemSettings.MaxTripDurationMinutes)
                {
                    return new ScanTicketResponseDto { IsValid = false, Message = $"Trip duration exceeded" };
                }

                // Update trip
                trip.ExitStationId = station.Id;
                trip.ExitTime = DateTime.UtcNow;
                trip.Status = TripStatus.Completed;

                // Update ticket status
                ticket.Status = TicketStatus.Used;

                // Save changes
                await _unitOfWork.SaveChangesAsync();

                return new ScanTicketResponseDto
                {
                    IsValid = true,
                    Message = successMessage,
                    TripSummary = new TripSummaryDto
                    {
                        EntryStationName = trip.EntryStation.Name,
                        ExitStationName = station.Name,
                        EntryTime = _timeService.FormatAsBdTime(trip.EntryTime),
                        ExitTime = _timeService.FormatAsBdTime(trip.ExitTime.Value)
                    }
                };
            }
            else
            {
                // Ticket is either pending, used, expired, or cancelled
                return defaultInvalidResponse;
            }
        }

        private async Task<ScanTicketResponseDto> HandleRapidPassScanAsync(Ticket ticket, Station station, SystemSettingsDto systemSettings, ScanTicketResponseDto defaultInvalidResponse)
        {
            // Check if RapidPass is Active
            if (ticket.Status == TicketStatus.Active)
            {
                // Check user wallet balance
                var userWalletBalance = await _walletService.GetBalanceByUserIdAsync(ticket.UserId);
                if (userWalletBalance < systemSettings.MinFare)
                {
                    return new ScanTicketResponseDto { IsValid = false, Message = "Insufficient Balance" };
                }

                // Create new trip
                var trip = new Trip
                {
                    UserId = ticket.UserId,
                    TicketId = ticket.Id,
                    EntryStationId = station.Id,
                    EntryTime = DateTime.UtcNow,
                    Status = TripStatus.InProgress
                };

                await _unitOfWork.TripRepository.CreateTripAsync(trip); // Save trip
                ticket.Status = TicketStatus.InUse;   // Update ticket status
                await _unitOfWork.SaveChangesAsync(); // Save changes

                return new ScanTicketResponseDto { IsValid = true, Message = successMessage };
            }
            // For exit scan with RapidPass
            else if (ticket.Status == TicketStatus.InUse)
            {
                // Find active(in-progress) trip
                var trip = await _unitOfWork.TripRepository.GetActiveTripByTicketIdAsync(ticket.Id);
                if (trip == null)
                {
                    return defaultInvalidResponse;
                }

                // Calculate fare
                decimal fareAmount = await _fareCalculationService.GetFareAsync(trip.EntryStationId, station.Id);

                // Description for transaction
                var description = $"Paid for Rapid Pass from {trip.EntryStation.Name} to {station.Name}";

                // Check time limit
                var tripDuration = DateTime.UtcNow - trip.EntryTime;
                if (tripDuration.TotalMinutes > systemSettings.MaxTripDurationMinutes)
                {
                    // Apply time limit penalty
                    fareAmount += systemSettings.TimeLimitPenaltyFee;
                    description += " (with penalty applied)";
                }

                // Process payment
                var paymentResult = await _paymentService.PayWithAccountBalanceAsync(ticket.UserId, fareAmount,PaymentItem.RapidPass, description);
                if (!paymentResult.Success)
                {
                    return new ScanTicketResponseDto { IsValid = false, Message = "Insufficient Balance" };
                }

                // Update trip
                trip.ExitStationId = station.Id;
                trip.ExitTime = DateTime.UtcNow;
                trip.FareAmount = fareAmount;
                trip.Status = TripStatus.Completed;
                trip.TransactionReference = paymentResult.TransactionReference;

                // Update ticket status
                ticket.Status = TicketStatus.Used;

                // Save changes
                await _unitOfWork.SaveChangesAsync();

                return new ScanTicketResponseDto
                {
                    IsValid = true,
                    Message = successMessage,
                    TripSummary = new TripSummaryDto
                    {
                        EntryStationName = trip.EntryStation.Name,
                        ExitStationName = station.Name,
                        EntryTime = _timeService.FormatAsBdTime(trip.EntryTime),
                        ExitTime = _timeService.FormatAsBdTime(trip.ExitTime.Value),
                        FareAmount = fareAmount
                    }
                };
            }
            else
            {
                // Ticket is either pending, used, expired, or cancelled
                return defaultInvalidResponse;
            }
        }
    }
}
