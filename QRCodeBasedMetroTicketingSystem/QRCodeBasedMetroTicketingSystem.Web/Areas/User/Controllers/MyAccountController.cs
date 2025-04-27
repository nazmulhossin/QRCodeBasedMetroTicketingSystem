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
    public class MyAccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly ITimeService _timeService;
        private readonly IMapper _mapper;
        public MyAccountController(IUserService userService, ITimeService timeService, IMapper mapper)
        {
            _userService = userService;
            _timeService = timeService;
            _mapper = mapper;
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        public async Task<IActionResult> MyProfile()
        {
            var userId = User.GetUserId();
            if (userId == null)
                return Unauthorized();

            var user = await _userService.GetUserByIdAsync(userId.Value);
            if (user == null)
                return NotFound();

            var viewModel = _mapper.Map<UserProfileViewModel>(user);
            viewModel.CreatedAt = _timeService.ConvertUtcToBdTime(viewModel.CreatedAt);

            return View(viewModel);
        }

        public async Task<IActionResult> TripHistory()
        {
            return View();
        }
    }
}
