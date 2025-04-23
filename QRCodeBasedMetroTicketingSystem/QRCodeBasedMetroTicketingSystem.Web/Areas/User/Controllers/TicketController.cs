using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QRCodeBasedMetroTicketingSystem.Application.Extensions;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services;
using QRCodeBasedMetroTicketingSystem.Domain.Entities;
using QRCodeBasedMetroTicketingSystem.Web.Areas.User.ViewModels;
using System.Runtime.InteropServices;

namespace QRCodeBasedMetroTicketingSystem.Web.Areas.User.Controllers
{
    [Area("User")]
    [Authorize(Roles = "User")]
    public class TicketController : Controller
    {
        private readonly ITicketService _ticketService;
        private readonly IQRCodeService _qrCodeService;
        private readonly IMapper _mapper;

        public TicketController(ITicketService ticketService, IQRCodeService qrCodeService, IMapper mapper)
        {
            _ticketService = ticketService;
            _qrCodeService = qrCodeService;
            _mapper = mapper;
        }

        public async Task<IActionResult> MyQrTickets(TicketStatus status = TicketStatus.Active)
        {
            var userId = User.GetUserId();
            if (userId == null)
                return Unauthorized();

            var tickets = await _ticketService.GetQrTicketsByStatusAsync(userId.Value, status);
            var viewModel = _mapper.Map<IEnumerable<TicketViewModel>>(tickets);

            return View(viewModel);
        }

        public async Task<IActionResult> TicketSummary(BuyTicketViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var summary = await _ticketService.GetTicketSummaryAsync(model.OriginStationId, model.DestinationStationId);
            var viewModel = new TicketSummaryViewModel
            {
                OriginStationId = model.OriginStationId,
                DestinationStationId = model.DestinationStationId,
                OriginStationName = summary.OriginStationName,
                DestinationStationName = summary.DestinationStationName,
                Fare = summary.Fare
            };

            return View(viewModel);
        }

        public IActionResult SelectPaymentOption(BuyTicketViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return View(model);
        }

        public async Task<IActionResult> ContinuePayment(PaymentOptionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.GetUserId();
            if (userId == null)
                return Unauthorized();

            // Purchase QR ticket
            var (transactionReference, amount) = await _ticketService.InitiatePurchaseQRTicketAsync(
                userId.Value,
                model.OriginStationId,
                model.DestinationStationId,
                model.PaymentOption.ToString());

            if (model.PaymentOption == PaymentOption.AccountBalance)
            {
                return RedirectToAction("ConfirmPurchase", new { transactionReference });
            }

            return RedirectToAction("EnterPaymentDetails", "Payment", new { transactionReference, amount });
        }

        public async Task<IActionResult> ConfirmPurchase(string transactionReference)
        {
            var isSuccess = await _ticketService.CompleteQRTicketPurchaseAsync(transactionReference);
            if(isSuccess)
            {
                return RedirectToAction("TicketPurchaseSuccessful", new { transactionReference });
            }

            return RedirectToAction("TicketPurchaseFailed", new { transactionReference });
        }

        public async Task<IActionResult> TicketPurchaseSuccessful(string transactionReference)
        {
            var ticket = await _ticketService.GetTicketByReferenceAsync(transactionReference);
            var viewModel = _mapper.Map<PurchaseStatusViewModel>(ticket);
            viewModel.TransactionId = transactionReference;

            return View(viewModel);
        }

        public async Task<IActionResult> TicketPurchaseFailed(string transactionReference)
        {
            var ticket = await _ticketService.GetTicketByReferenceAsync(transactionReference);
            var viewModel = _mapper.Map<PurchaseStatusViewModel>(ticket);

            return View(viewModel);
        }

        public async Task<IActionResult> GetTicketDetailsWithQRCode(int ticketId)
        {
            var userId = User.GetUserId();
            if (userId == null)
                return Unauthorized();

            var ticket = await _ticketService.GetTicketByIdAsync(ticketId);
            if (ticket == null || ticket.UserId != userId)
            {
                return Unauthorized();
            }

            var qrCodeBase64 = _qrCodeService.GenerateQRCode(ticket.QRCodeData);

            return Json(new
            {
                ticketId = ticket.Id,
                qrCodeImage = $"data:image/png;base64,{qrCodeBase64}",
                originStationName = ticket.OriginStationName,
                destinationStationName = ticket.DestinationStationName,
                expiryTime = ConvertToBangladeshTime(ticket.ExpiryTime).ToString("MMM d, yyyy h:mm tt")
            });
        }

        private DateTime ConvertToBangladeshTime(DateTime utcDateTime)
        {
            try
            {
                if (utcDateTime.Kind != DateTimeKind.Utc)
                {
                    utcDateTime = DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc);
                }

                var timeZoneId = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                    ? "Bangladesh Standard Time"
                    : "Asia/Dhaka";

                var bstTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
                return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, bstTimeZone);
            }
            catch (TimeZoneNotFoundException)
            {
                return utcDateTime.AddHours(6);
            }
        }
    } 
}
