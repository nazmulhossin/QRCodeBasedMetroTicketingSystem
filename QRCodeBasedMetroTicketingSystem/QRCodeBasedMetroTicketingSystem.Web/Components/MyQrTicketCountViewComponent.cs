using Microsoft.AspNetCore.Mvc;
using QRCodeBasedMetroTicketingSystem.Application.Extensions;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services;
using System.Security.Claims;

namespace QRCodeBasedMetroTicketingSystem.Web.Components
{
    public class MyQrTicketCountViewComponent : ViewComponent
    {
        private readonly ITicketService _ticketService;

        public MyQrTicketCountViewComponent(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userId = (User as ClaimsPrincipal)?.GetUserId();
            if (userId == null)
                return View("Default", 0);

            var myQrTicketsCount = await _ticketService.GetActiveAndInUseTicketsCountAsync(userId.Value);
            return View("Default", myQrTicketsCount);
        }
    }
}
