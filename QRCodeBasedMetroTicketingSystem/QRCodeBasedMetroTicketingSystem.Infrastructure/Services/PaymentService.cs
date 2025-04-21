using AutoMapper;
using QRCodeBasedMetroTicketingSystem.Application.DTOs;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Repositories;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services;
using QRCodeBasedMetroTicketingSystem.Domain.Entities;

namespace QRCodeBasedMetroTicketingSystem.Infrastructure.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ITicketService _ticketService;

        public PaymentService(IUnitOfWork unitOfWork, IMapper mapper, ITicketService ticketService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _ticketService = ticketService;
        }

        public async Task<string> InitiatePaymentAsync(int userId, decimal amount, PaymentMethod paymentMethod, string? transactionReference)
        {
            // A non-null transactionReference indicates that a transaction has already been initiated for purchasing a QR ticket
            if (transactionReference != null)
            {
                var existTransaction = await _unitOfWork.TransactionRepository.GetByReferenceAsync(transactionReference);
                existTransaction.PaymentMethod = paymentMethod;
                await _unitOfWork.SaveChangesAsync();

                return transactionReference;
            }

            // Get or create wallet
            var wallet = await _unitOfWork.WalletRepository.GetByUserIdAsync(userId);

            if (wallet == null)
            {
                wallet = new Wallet { UserId = userId };
                await _unitOfWork.WalletRepository.CreateAsync(wallet);
                await _unitOfWork.SaveChangesAsync();
            }

            // Create a pending transaction
            var transaction = new Transaction
            {
                WalletId = wallet.Id,
                Amount = amount,
                Type = TransactionType.TopUp,
                PaymentMethod = paymentMethod,
                Status = TransactionStatus.Pending,
                TransactionReference = Guid.NewGuid().ToString(),
                Description = $"Top-up ৳{amount} via {paymentMethod}"
            };

            await _unitOfWork.TransactionRepository.CreateAsync(transaction);
            await _unitOfWork.SaveChangesAsync();

            // In the real application, here we should integrate with the actual payment gateway (For bKash, Nagad, or card payment)
            // For demo purposes, we'll just return the transaction reference
            return transaction.TransactionReference;
        }

        public async Task<bool> VerifyPaymentAsync(string transactionReference, PaymentMethod paymentMethod)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // In a real application, we should verify the payment with the payment gateway
                // For demo purposes, we will just simulate a successful payment
                var transaction = await _unitOfWork.TransactionRepository.GetByReferenceAsync(transactionReference);

                if (transaction == null)
                {
                    return false;
                }

                if (transaction.PaymentFor == PaymentItem.QRTicket)
                {
                    // If the payment is for QR ticket manage transaction at ticket service
                    var isSuccess = await _ticketService.CompleteQRTicketPurchaseAsync(transactionReference);

                    if (!isSuccess)
                    {
                        await _unitOfWork.RollbackTransactionAsync();
                        return false;
                    }   
                }
                else
                {
                    // Update transaction status
                    transaction.Status = TransactionStatus.Completed;
                    var wallet = await _unitOfWork.WalletRepository.GetByIdAsync(transaction.WalletId);
                    if (wallet == null)
                    {
                        await _unitOfWork.RollbackTransactionAsync();
                        return false;
                    }
                    // Update wallet balance
                    wallet.Balance += transaction.Amount;
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return true;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                return false;
            }
        }

        public async Task<bool> CancelPaymentAsync(string transactionReference)
        {
            try
            {
                var transaction = await _unitOfWork.TransactionRepository.GetByReferenceAsync(transactionReference);
                if (transaction == null)
                {
                    return false;
                }

                // Only allow cancellation if the transaction is still pending
                if (transaction.Status != TransactionStatus.Pending)
                {
                    return false;
                }

                // Update transaction status to cancelled
                transaction.Status = TransactionStatus.Canceled;
                transaction.Description += " (Canceled)";

                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<TransactionDto?> GetTransactionByReferenceAsync(string transactionReference)
        {
            var result = await _unitOfWork.TransactionRepository.GetByReferenceAsync(transactionReference);
            return _mapper.Map<TransactionDto>(result);
        }
    }
}
