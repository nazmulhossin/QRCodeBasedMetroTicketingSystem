using AutoMapper;
using QRCodeBasedMetroTicketingSystem.Application.DTOs;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Repositories;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services;
using QRCodeBasedMetroTicketingSystem.Domain.Entities;

namespace QRCodeBasedMetroTicketingSystem.Infrastructure.Services
{
    public class TicketService : ITicketService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFareCalculationService _fareCalculationService;
        private readonly IStationService _stationService;
        private readonly ISystemSettingsService _systemSettingsService;
        private readonly IWalletService _walletService;
        private readonly IQRCodeService _qrCodeService;
        private readonly IMapper _mapper;

        public TicketService(
            IUnitOfWork unitOfWork,
            IFareCalculationService fareCalculationService,
            IStationService stationService,
            ISystemSettingsService systemSettingsService,
            IWalletService walletService,
            IQRCodeService qrCodeService,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _fareCalculationService = fareCalculationService;
            _stationService = stationService;
            _systemSettingsService = systemSettingsService;
            _walletService = walletService;
            _qrCodeService = qrCodeService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TicketDto>> GetQrTicketsByStatusAsync(int userId, TicketStatus status)
        {
            var tickets = await _unitOfWork.TicketRepository.GetQrTicketsByStatusAsync(userId, status);
            return _mapper.Map<IEnumerable<TicketDto>>(tickets);
        }

        public async Task<TicketDto?> GetTicketByIdAsync(int ticketId)
        {
            var tickets = await _unitOfWork.TicketRepository.GetTicketByIdAsync(ticketId);
            return _mapper.Map<TicketDto>(tickets);
        }

        public async Task<int> GetActiveAndInUseTicketsCountAsync(int userId)
        {
            return await _unitOfWork.TicketRepository.GetActiveAndInUseTicketsCountAsync(userId);
        }

        public async Task<TicketDto?> GetActiveRapidPassTicketByUserIdAsync(int userId)
        {
            var tickets = await _unitOfWork.TicketRepository.GetActiveRapidPassTicketByUserIdAsync(userId);
            return _mapper.Map<TicketDto>(tickets);
        }

        public async Task<(string OriginStationName, string DestinationStationName, int Fare)> GetTicketSummaryAsync(int originStationId, int destinationStationId)
        {
            var originStation = await _unitOfWork.StationRepository.GetStationByIdAsync(originStationId);
            var destinationStation = await _unitOfWork.StationRepository.GetStationByIdAsync(destinationStationId);
            var fare = await _fareCalculationService.GetFareAsync(originStationId, destinationStationId);

            return (originStation.Name, destinationStation.Name, fare);
        }

        public async Task<(string TransactionReference, decimal Amount)> InitiatePurchaseQRTicketAsync(int userId, int originStationId, int destinationStationId, string paymentOption)
        {
            var systemSettings = await _systemSettingsService.GetSystemSettingsAsync();
            int fare = await _fareCalculationService.GetFareAsync(originStationId, destinationStationId);
            var originStation = await _unitOfWork.StationRepository.GetStationByIdAsync(originStationId);
            var destinationStation = await _unitOfWork.StationRepository.GetStationByIdAsync(destinationStationId);
            var wallet = await _walletService.GetWalletByUserIdAsync(userId);

            // Create new transaction and make it pending
            var transaction = new Transaction
            {
                WalletId = wallet.Id,
                Amount = fare,
                Type = TransactionType.Payment,
                PaymentFor = PaymentItem.QRTicket,
                Status = TransactionStatus.Pending,
                TransactionReference = Guid.NewGuid().ToString(),
                Description = $"QR Ticket purchase from {originStation.Name} to {destinationStation.Name}"
            };

            // Handle payment based on payment method
            if (paymentOption == PaymentMethod.AccountBalance.ToString())
            {
                transaction.PaymentMethod = PaymentMethod.AccountBalance;
            }

            // Save transaction to database
            await _unitOfWork.TransactionRepository.CreateAsync(transaction);

            // Create new ticket
            var ticket = new Ticket
            {
                UserId = userId,
                Type = TicketType.QRTicket,
                OriginStationId = originStationId,
                DestinationStationId = destinationStationId,
                FareAmount = fare,
                Status = TicketStatus.Pending,
                ExpiryTime = DateTime.UtcNow.AddMinutes(systemSettings.QrTicketValidityMinutes),
                TransactionReference = transaction.TransactionReference
            };

            // Generate QR code data
            ticket.QRCodeData = _qrCodeService.GenerateQRCodeData(ticket);

            // Save ticket to database
            await _unitOfWork.TicketRepository.CreateTicketAsync(ticket);
            await _unitOfWork.SaveChangesAsync();

            // Update the QR code with the actual ticket ID
            ticket.QRCodeData = _qrCodeService.GenerateQRCodeData(ticket);
            await _unitOfWork.SaveChangesAsync();

            return (transaction.TransactionReference, fare);
        }

        public async Task<bool> CompleteQRTicketPurchaseAsync(string transactionReference)
        {
            var systemSettings = await _systemSettingsService.GetSystemSettingsAsync();
            var ticket = await _unitOfWork.TicketRepository.GetByReferenceAsync(transactionReference);
            var transaction = await _unitOfWork.TransactionRepository.GetByReferenceAsync(transactionReference);

            if (ticket == null || transaction == null)
            {
                return false;
            }

            if (transaction.PaymentMethod == PaymentMethod.AccountBalance)
            {
                var isSuccrss = await _walletService.DeductBalanceAsync(ticket.UserId, ticket.FareAmount.Value);
                if (!isSuccrss)
                {
                    return false;
                }
            }

            ticket.Status = TicketStatus.Active;
            transaction.Status = TransactionStatus.Completed;

            if (transaction.PaymentMethod == PaymentMethod.AccountBalance)
            {
                await _unitOfWork.SaveChangesAsync();
            }

            return true;
        }

        public async Task<TicketDto?> GetTicketByReferenceAsync(string transactionReference)
        {
            var ticket = await _unitOfWork.TicketRepository.GetByReferenceAsync(transactionReference);
            var originStation = await _unitOfWork.StationRepository.GetStationByIdAsync(ticket.OriginStationId.Value);
            var destinationStation = await _unitOfWork.StationRepository.GetStationByIdAsync(ticket.DestinationStationId.Value);

            // Convert into TicketDto
            var ticketDto = _mapper.Map<TicketDto>(ticket);
            ticketDto.OriginStationName = originStation.Name;
            ticketDto.DestinationStationName = destinationStation.Name;

            return ticketDto;
        }
    }
}
