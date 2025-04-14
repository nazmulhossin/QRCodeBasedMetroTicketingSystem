using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services;

namespace QRCodeBasedMetroTicketingSystem.Web.Areas.User.Controllers
{
    [Area("User")]
    [Authorize(Roles = "User")]
    public class WalletController : Controller
    {
        private readonly IWalletService _walletService;
        private readonly IPaymentService _paymentService;
        private readonly IUserService _userService;

        public WalletController(IWalletService walletService, IPaymentService paymentService, IUserService userService)
        {
            _walletService = walletService;
            _paymentService = paymentService;
            _userService = userService;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
