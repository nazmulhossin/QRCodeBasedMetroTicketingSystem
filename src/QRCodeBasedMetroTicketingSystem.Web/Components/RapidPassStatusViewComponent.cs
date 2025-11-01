using Microsoft.AspNetCore.Mvc;
using QRCodeBasedMetroTicketingSystem.Application.DTOs;
using QRCodeBasedMetroTicketingSystem.Application.Extensions;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services;
using System.Security.Claims;

namespace QRCodeBasedMetroTicketingSystem.Web.Components
{
    public class RapidPassStatusViewComponent : ViewComponent
    {
        private readonly ITicketService _ticketService;

        public RapidPassStatusViewComponent(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            TicketDto? ticket = new();
            var userId = (User as ClaimsPrincipal)?.GetUserId();
            if (userId != null)
            {
                ticket = await _ticketService.GetActiveRapidPassAsync(userId.Value) ?? new TicketDto();
            }

            return View(ticket);
        }
    }
}
