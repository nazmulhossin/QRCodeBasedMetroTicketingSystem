using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QRCodeBasedMetroTicketingSystem.Application.Extensions;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services;
using QRCodeBasedMetroTicketingSystem.Domain.Entities;
using QRCodeBasedMetroTicketingSystem.Web.Areas.User.ViewModels;

namespace QRCodeBasedMetroTicketingSystem.Web.Areas.User.Controllers
{
    [Area("user")]
    [Authorize(Roles = "User")]
    public class PaymentController : Controller
    {
        private readonly IPaymentService _paymentService;
        private readonly IMapper _mapper;

        public PaymentController(IPaymentService paymentService, IMapper mapper)
        {
            _paymentService = paymentService;
            _mapper = mapper;
        }

        public IActionResult EnterPaymentDetails(string? transactionReference, decimal amount = 100)
        {
            var viewModel = new PaymentDetailsViewModel
            {
                Amount = amount,
                PaymentMethod = PaymentMethod.BKash
            };

            if (transactionReference != null)
            {
                viewModel.TransactionReference = transactionReference;
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnterPaymentDetails(PaymentDetailsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = User.GetUserId();
            if (userId == null)
                return Unauthorized();

            var transactionReference = await _paymentService.InitiatePaymentAsync(userId.Value, model.Amount, model.PaymentMethod, model.TransactionReference);

            // Redirect to payment gateway based on the selected payment method
            return RedirectToAction("ProcessPayment", new { transactionReference });
        }

        public async Task<IActionResult> ProcessPayment(string transactionReference)
        {
            if (string.IsNullOrEmpty(transactionReference))
            {
                return RedirectToAction("Index", "Home");
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

            return RedirectToAction("Index", "Home");
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
