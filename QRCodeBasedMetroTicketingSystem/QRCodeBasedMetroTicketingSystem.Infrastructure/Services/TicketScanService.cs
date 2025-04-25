using Azure;
using Microsoft.Extensions.Configuration;
using QRCodeBasedMetroTicketingSystem.Application.DTOs;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Repositories;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services;
using QRCodeBasedMetroTicketingSystem.Domain.Entities;
using QRCodeBasedMetroTicketingSystem.Infrastructure.Repositories;
using static System.Collections.Specialized.BitVector32;

namespace QRCodeBasedMetroTicketingSystem.Infrastructure.Services
{
    public class TicketScanService : ITicketScanService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IWalletService _walletService;
        private readonly IQRCodeService _qrCodeService;
        private readonly IFareCalculationService _fareCalculationService;
        private readonly ISystemSettingsService _systemSettingsService;

        public TicketScanService(
            IUnitOfWork unitOfWork,
            IConfiguration configuration,
            IWalletService walletService,
            IQRCodeService qrCodeService,
            IFareCalculationService fareCalculationService,
            ISystemSettingsService systemSettingsService)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _walletService = walletService;
            _qrCodeService = qrCodeService;
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

            var (ticketId, expiryTimestamp) = _qrCodeService.ParseQRCodeData(request.QRCodeData);

            // Check if ticket is expired
            var expiryTime = new DateTime(expiryTimestamp);
            if (DateTime.UtcNow > expiryTime)
            {
                return new ScanTicketResponseDto { IsValid = false, Message = "Ticket has expired" };
            }

            var ticket = await _unitOfWork.TicketRepository.GetTicketByIdAsync(ticketId);
            if (ticket == null)
            {
                return defaultInvalidResponse;
            }

            // Validate QRCodeData with server data
            if (ticket.ExpiryTime < DateTime.UtcNow)
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
            var settings = await _unitOfWork.SystemSettingsRepository.GetSystemSettingsAsync();
            if (settings == null)
            {
                return defaultInvalidResponse;
            }

            // Handle both ticket types
            if (ticket.Type == TicketType.QRTicket)
            {
                return await HandleQRTicketScanAsync(ticket, station, settings);
            }
            else // RapidPass
            {
                return await HandleRapidPassScanAsync(ticket, station, settings);
            }
        }

        private async Task<ScanTicketResponseDto> HandleQRTicketScanAsync(Ticket ticket, Station station, SystemSettings settings)
        {
            // If the ticket is Active, this is an entry scan
            if (ticket.Status == TicketStatus.Active)
            {
                // Create new trip
                var trip = new Trip
                {
                    UserId = ticket.UserId,
                    TicketId = ticket.Id,
                    EntryStationId = station.Id,
                    EntryTime = DateTime.UtcNow,
                    Status = TripStatus.InProgress
                };

                // Update ticket status
                ticket.Status = TicketStatus.InUse;

                // Save changes
                await _ticketRepository.UpdateAsync(ticket);
                await _unitOfWork.AddAsync(trip);
                await _unitOfWork.SaveChangesAsync();

                return new ScanTicketResponseDto
                {
                    IsValid = true,
                    Message = "Entry successful"
                };
            }
            // If the ticket is InUse, this is an exit scan
            else if (ticket.Status == TicketStatus.InUse)
            {
                // Find active trip
                var trip = await _ticketRepository.GetActiveTrip(ticket.Id);
                if (trip == null)
                {
                    return new ScanTicketResponseDto { IsValid = false, Message = "No active trip found for this ticket" };
                }

                // Check time limit
                var tripDuration = DateTime.UtcNow - trip.EntryTime;
                if (tripDuration.TotalMinutes > settings.MaxTripDurationMinutes)
                {
                    // Apply penalty logic if implemented
                    return new ScanTicketResponseDto { IsValid = false, Message = $"Trip duration exceeded maximum allowed time of {settings.MaxTripDurationMinutes} minutes" };
                }

                // For QR Ticket, fare is prepaid
                var entryStation = await _stationRepository.GetByIdAsync(trip.EntryStationId);

                // Update trip
                trip.ExitStationId = station.Id;
                trip.ExitTime = DateTime.UtcNow;
                trip.Status = TripStatus.Completed;

                // Update ticket status
                ticket.Status = TicketStatus.Used;

                // Save changes
                await _ticketRepository.UpdateAsync(ticket);
                await _unitOfWork.UpdateAsync(trip);
                await _unitOfWork.SaveChangesAsync();

                return new ScanTicketResponseDto
                {
                    IsValid = true,
                    Message = "Exit successful",
                    TripSummary = new TripSummaryDto
                    {
                        OriginStationName = entryStation.Name,
                        DestinationStationName = station.Name,
                        EntryTime = trip.EntryTime,
                        ExitTime = trip.ExitTime.Value,
                        FareAmount = ticket.FareAmount.Value
                    }
                };
            }
            else
            {
                // Ticket is either pending, used, expired, or cancelled
                return new ScanTicketResponseDto
                {
                    IsValid = false,
                    Message = $"Ticket is in invalid state: {ticket.Status}"
                };
            }
        }

        private async Task<ScanTicketResponseDto> HandleRapidPassScanAsync(Ticket ticket, Station station, SystemSettings settings)
        {
            // Check if RapidPass is Active
            if (ticket.Status == TicketStatus.Active)
            {
                // Check user wallet balance
                var userWallet = await _walletService.GetWalletByUserIdAsync(ticket.UserId);
                if (userWallet.Balance < settings.MinFare)
                {
                    return new ScanTicketResponseDto { IsValid = false, Message = "Insufficient balance for entry" };
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

                // Update ticket status
                ticket.Status = TicketStatus.InUse;

                // Save changes
                await _ticketRepository.UpdateAsync(ticket);
                await _unitOfWork.AddAsync(trip);
                await _unitOfWork.SaveChangesAsync();

                return new ScanTicketResponseDto
                {
                    IsValid = true,
                    Message = "Entry successful"
                };
            }
            // For exit scan with RapidPass
            else if (ticket.Status == TicketStatus.InUse)
            {
                // Find active trip
                var trip = await _ticketRepository.GetActiveTrip(ticket.Id);
                if (trip == null)
                {
                    return new ScanTicketResponseDto { IsValid = false, Message = "No active trip found for this pass" };
                }

                // Check time limit
                var tripDuration = DateTime.UtcNow - trip.EntryTime;
                if (tripDuration.TotalMinutes > settings.MaxTripDurationMinutes)
                {
                    // Apply time limit penalty
                    // Implementation depends on your business logic
                }

                // Calculate fare
                var entryStation = await _stationRepository.GetByIdAsync(trip.EntryStationId);
                decimal fareAmount = await _fareCalculationService.CalculateFareAsync(trip.EntryStationId, station.Id);

                // Check if user has enough balance
                var userWallet = await _walletService.GetWalletByUserIdAsync(ticket.UserId);
                if (userWallet.Balance < fareAmount)
                {
                    return new ScanTicketResponseDto { IsValid = false, Message = "Insufficient balance for exit" };
                }

                // Process payment
                var paymentResult = await _walletService.ProcessPaymentAsync(
                    ticket.UserId,
                    fareAmount,
                    PaymentItem.RapidPass,
                    $"Trip from {entryStation.Name} to {station.Name}"
                );

                if (!paymentResult.Success)
                {
                    return new ScanTicketResponseDto { IsValid = false, Message = paymentResult.Message };
                }

                // Update trip
                trip.ExitStationId = station.Id;
                trip.ExitTime = DateTime.UtcNow;
                trip.FareAmount = fareAmount;
                trip.Status = TripStatus.Completed;
                trip.TransactionReference = paymentResult.TransactionReference;

                // Update ticket status back to Active for future use
                ticket.Status = TicketStatus.Active;

                // Save changes
                await _ticketRepository.UpdateAsync(ticket);
                await _unitOfWork.UpdateAsync(trip);
                await _unitOfWork.SaveChangesAsync();

                return new ScanTicketResponseDto
                {
                    IsValid = true,
                    Message = "Exit successful",
                    TripSummary = new TripSummaryDto
                    {
                        OriginStationName = entryStation.Name,
                        DestinationStationName = station.Name,
                        EntryTime = trip.EntryTime,
                        ExitTime = trip.ExitTime.Value,
                        FareAmount = fareAmount
                    }
                };
            }
            else
            {
                // Ticket is either pending, used, expired, or cancelled
                return new ScanTicketResponseDto
                {
                    IsValid = false,
                    Message = $"Rapid Pass is in invalid state: {ticket.Status}"
                };
            }
        }
    }
}
