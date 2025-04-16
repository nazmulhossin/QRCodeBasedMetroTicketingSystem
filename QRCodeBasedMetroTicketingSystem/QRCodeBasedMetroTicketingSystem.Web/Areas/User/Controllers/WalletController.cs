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
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public WalletController(IWalletService walletService, IPaymentService paymentService, IUserService userService, IMapper mapper)
        {
            _walletService = walletService;
            _paymentService = paymentService;
            _userService = userService;
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

            var wallet = await _walletService.GetWalletByUserIdAsync(userId.Value);
            return Json(new { balance = wallet.Balance });
        }

        public IActionResult AddBalance()
        {
            return View();
        }
    }
}
