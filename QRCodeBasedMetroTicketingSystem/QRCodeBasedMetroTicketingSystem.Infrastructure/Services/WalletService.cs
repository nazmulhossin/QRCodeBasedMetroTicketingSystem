using AutoMapper;
using QRCodeBasedMetroTicketingSystem.Application.DTOs;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Repositories;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services;
using QRCodeBasedMetroTicketingSystem.Domain.Entities;

namespace QRCodeBasedMetroTicketingSystem.Infrastructure.Services
{
    public class WalletService : IWalletService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public WalletService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<WalletDto> GetWalletByUserIdAsync(int userId)
        {
            var wallet = await _unitOfWork.WalletRepository.GetByUserIdAsync(userId);

            if (wallet == null)
            {
                // Create a new wallet if it doesn't exist
                wallet = new Wallet { UserId = userId };
                await _unitOfWork.WalletRepository.CreateAsync(wallet);
                await _unitOfWork.SaveChangesAsync();
            }

            var transactions = await _unitOfWork.TransactionRepository.GetByWalletIdAsync(wallet.Id, 10);
            var walletDto = _mapper.Map<WalletDto>(wallet);
            walletDto.RecentTransactions = _mapper.Map<List<TransactionDto>>(transactions);

            return walletDto;
        }

        public async Task<decimal> GetBalanceByUserIdAsync(int userId)
        {
            var wallet = await _unitOfWork.WalletRepository.GetByUserIdAsync(userId);

            if (wallet == null)
            {
                // Create a new wallet if it doesn't exist
                wallet = new Wallet { UserId = userId };
                await _unitOfWork.WalletRepository.CreateAsync(wallet);
            }

            return wallet.Balance;
        }

        public async Task<bool> DeductBalanceAsync(int userId, decimal amount)
        {
            var wallet = await _unitOfWork.WalletRepository.GetByUserIdAsync(userId);

            if (wallet == null || wallet.Balance < amount)
            {
                return false;
            }

            // Update wallet balance
            wallet.Balance -= amount;
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
