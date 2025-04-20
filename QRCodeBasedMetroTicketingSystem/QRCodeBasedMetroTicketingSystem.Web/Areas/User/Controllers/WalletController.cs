using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QRCodeBasedMetroTicketingSystem.Application.Extensions;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services;
using QRCodeBasedMetroTicketingSystem.Web.Areas.User.ViewModels;

namespace QRCodeBasedMetroTicketingSystem.Web.Areas.User.Controllers
{
    [Area("User")]
    [Authorize(Roles = "User")]
    public class WalletController : Controller
    {
        private readonly IWalletService _walletService;
        private readonly IPaymentService _paymentService;
        private readonly IMapper _mapper;

        public WalletController(IWalletService walletService, IPaymentService paymentService, IMapper mapper)
        {
            _walletService = walletService;
            _paymentService = paymentService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.GetUserId();
            if (userId == null)
                return Unauthorized();

            var wallet = await _walletService.GetWalletByUserIdAsync(userId.Value);
            var viewModel = _mapper.Map<WalletViewModel>(wallet);

            return View(viewModel);
        }

        public async Task<IActionResult> GetBalance()
        {
            var userId = User.GetUserId();
            if (userId == null)
                return Unauthorized();

            var balance = await _walletService.GetBalanceByUserIdAsync(userId.Value);
            return Json(new { balance });
        }

        public IActionResult AddBalance()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddBalance(AddBalanceViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = User.GetUserId();
            if (userId == null)
                return Unauthorized();

            // Initiate payment process
            var transactionReference = await _paymentService.InitiatePaymentAsync(userId.Value, model.Amount, model.PaymentMethod);

            // Redirect to payment gateway based on the selected payment method
            return RedirectToAction("ProcessPayment", new { transactionReference });
        }

        public async Task<IActionResult> ProcessPayment(string transactionReference)
        {
            if (string.IsNullOrEmpty(transactionReference))
            {
                return RedirectToAction("AddBalance");
            }

            // In the real application, we should redirect to the actual payment gateway
            // For demo purposes, we will just show a payment simulation view
            var transaction = await _paymentService.GetTransactionByReferenceAsync(transactionReference);
            var viewModels = _mapper.Map<TransactionViewModel>(transaction);

            return View(viewModels);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompletePayment(CompletePaymentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("PaymentFailed");
            }

            // Verify payment with payment gateway
            var success = await _paymentService.VerifyPaymentAsync(model.TransactionReference, model.PaymentMethod);
            if (success)
            {
                return RedirectToAction("PaymentSuccess", new { model.TransactionReference });
            }

            return RedirectToAction("PaymentFailed");
        }

        public async Task<IActionResult> CancelPayment(string transactionReference)
        {
            // Cancel pending transaction
            var success = await _paymentService.CancelPaymentAsync(transactionReference);

            if (success)
            {
                return RedirectToAction("AddBalance");
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> PaymentSuccess(string transactionReference)
        {
            if (string.IsNullOrEmpty(transactionReference))
            {
                return RedirectToAction("PaymentFailed");
            }

            var transaction = await _paymentService.GetTransactionByReferenceAsync(transactionReference);
            var viewModels = _mapper.Map<TransactionViewModel>(transaction);

            return View(viewModels);
        }

        public IActionResult PaymentFailed()
        {
            return View();
        }
    }
}
