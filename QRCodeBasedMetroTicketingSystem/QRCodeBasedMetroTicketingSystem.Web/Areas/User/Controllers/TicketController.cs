using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services;
using QRCodeBasedMetroTicketingSystem.Web.Areas.User.ViewModels;

namespace QRCodeBasedMetroTicketingSystem.Web.Areas.User.Controllers
{
    [Area("User")]
    [Authorize(Roles = "User")]
    public class TicketController : Controller
    {
        private readonly ITicketService _ticketService;

        public TicketController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        public IActionResult MyQrTickets()
        {
            return View();
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

        public async Task<IActionResult> SelectPaymentMethod()
        {
            return View();
        }
    } 
}
