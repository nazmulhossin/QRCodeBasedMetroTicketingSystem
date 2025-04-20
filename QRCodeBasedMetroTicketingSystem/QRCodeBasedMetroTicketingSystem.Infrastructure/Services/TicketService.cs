using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Repositories;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services;

namespace QRCodeBasedMetroTicketingSystem.Infrastructure.Services
{
    public class TicketService : ITicketService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFareCalculationService _fareCalculationService;
        private readonly IWalletService _walletService;
        private readonly IPaymentService _paymentService;
        private readonly ISystemSettingsService _systemSettingsService;

        public TicketService(
            IUnitOfWork unitOfWork,
            IFareCalculationService fareCalculationService,
            IWalletService walletService,
            IPaymentService paymentService,
            ISystemSettingsService systemSettingsService)
        {
            _unitOfWork = unitOfWork;
            _fareCalculationService = fareCalculationService;
            _walletService = walletService;
            _paymentService = paymentService;
            _systemSettingsService = systemSettingsService;
        }

        public async Task<(string OriginStationName, string DestinationStationName, int Fare)> GetTicketSummaryAsync(int OriginStationId, int DestinationStationId)
        {
            var originStation = await _unitOfWork.StationRepository.GetStationByIdAsync(OriginStationId);
            var destinationStation = await _unitOfWork.StationRepository.GetStationByIdAsync(DestinationStationId);
            var fare = await _fareCalculationService.GetFareAsync(OriginStationId, DestinationStationId);

            return (originStation.Name, destinationStation.Name, fare);
        }
    }
}
