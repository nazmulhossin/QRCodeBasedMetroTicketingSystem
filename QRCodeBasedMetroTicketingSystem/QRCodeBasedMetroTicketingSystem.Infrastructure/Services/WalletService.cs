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

        public async Task<decimal> GetBalanceAsync(int userId)
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

        public async Task<bool> AddBalanceAsync(int userId, decimal amount, PaymentMethod paymentMethod, string transactionReference)
        {
            var wallet = await _unitOfWork.WalletRepository.GetByUserIdAsync(userId);

            if (wallet == null)
            {
                wallet = new Wallet { UserId = userId };
                await _unitOfWork.WalletRepository.CreateAsync(wallet);
            }

            // Create a transaction record
            var transaction = new Transaction
            {
                WalletId = wallet.Id,
                Amount = amount,
                Type = TransactionType.TopUp,
                PaymentMethod = paymentMethod,
                Status = TransactionStatus.Completed,
                TransactionReference = transactionReference,
                Description = $"Added {amount:C} via {paymentMethod}"
            };

            await _unitOfWork.TransactionRepository.CreateAsync(transaction);

            // Update wallet balance
            wallet.Balance += amount;
            //await _unitOfWork.WalletRepository.UpdateAsync(wallet);

            return true;
        }

        public async Task<bool> DeductBalanceAsync(int userId, decimal amount, TransactionType type, string description)
        {
            var wallet = await _unitOfWork.WalletRepository.GetByUserIdAsync(userId);

            if (wallet == null || wallet.Balance < amount)
            {
                return false;
            }

            // Create a transaction record
            var transaction = new Transaction
            {
                WalletId = wallet.Id,
                Amount = amount,
                Type = type,
                PaymentMethod = PaymentMethod.System,
                Status = TransactionStatus.Completed,
                Description = description
            };

            await _unitOfWork.TransactionRepository.CreateAsync(transaction);

            // Update wallet balance
            wallet.Balance -= amount;
            //await _unitOfWork.WalletRepository.UpdateAsync(wallet);

            return true;
        }
    }
}
