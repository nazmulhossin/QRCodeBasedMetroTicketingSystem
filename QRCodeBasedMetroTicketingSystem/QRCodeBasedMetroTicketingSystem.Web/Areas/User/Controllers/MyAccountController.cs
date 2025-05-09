﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QRCodeBasedMetroTicketingSystem.Application.Extensions;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services;
using QRCodeBasedMetroTicketingSystem.Domain.Entities;
using QRCodeBasedMetroTicketingSystem.Web.Areas.User.ViewModels;

namespace QRCodeBasedMetroTicketingSystem.Web.Areas.User.Controllers
{
    [Area("User")]
    [Authorize(Roles = "User")]
    public class MyAccountController : Controller
    {
        private readonly IUserDashboardService _userDashboardService;
        private readonly IUserService _userService;
        private readonly ITripService _tripService;
        private readonly ITimeService _timeService;
        private readonly IMapper _mapper;
        public MyAccountController(IUserDashboardService userDashboardService,IUserService userService, ITripService tripService, ITimeService timeService, IMapper mapper)
        {
            _userDashboardService = userDashboardService;
            _userService = userService;
            _tripService = tripService;
            _timeService = timeService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Dashboard()
        {
            var userId = User.GetUserId();
            if (userId == null)
                return Unauthorized();

            var stats = await _userDashboardService.GetUserStatsAsync(userId.Value);
            var activities = await _userDashboardService.GetRecentActivitiesAsync(userId.Value);

            var viewModel = new UserDashboardViewModel
            {
                Stats = stats,
                Activities = activities
            };

            return View(viewModel);
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
            var userId = User.GetUserId();
            if (userId == null)
                return Unauthorized();

            var completedTrips = await _tripService.GetCompletedTripsByUserIdAsync(userId.Value);

            var viewModel = completedTrips.Select(trip => new TripViewModel
            {
                Id = $"TR-{trip.Id}",
                EntryStationName = trip.EntryStationName,
                ExitStationName = trip.ExitStationName,
                EntryTimeFormatted = _timeService.FormatAsBdTime(trip.EntryTime),
                ExitTimeFormatted = _timeService.FormatAsBdTime(trip.ExitTime),
                TicketType = trip.TicketType == TicketType.RapidPass ? "Rapid Pass" : "QR Ticket",
                Fare = trip.FareAmount
            }).ToList();

            return View(viewModel);
        }

        public async Task<IActionResult> SecuritySettings()
        {
            var userId = User.GetUserId();
            if (userId == null)
                return Unauthorized();

            var lastPasswordChangeDate = await _userService.GetLastPasswordChangeDateAsync(userId.Value);
            var viewModel = new SecuritySettingsViewModel { LastPasswordChangeDateFormatted = _timeService.FormatAsBdTime(lastPasswordChangeDate, "dd MMMM yyyy") };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            var userId = User.GetUserId();
            if (userId == null)
                return Unauthorized();

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Invalid data. Please try again.";
                return RedirectToAction("SecuritySettings");
            }

            var result = await _userService.ChangePasswordAsync(userId.Value, model.CurrentPassword, model.NewPassword);
            if (result.IsSuccess)
                TempData["SuccessMessage"] = result.Message;
            else
                TempData["ErrorMessage"] = result.Message;

            return RedirectToAction("SecuritySettings");
        }
    }
}
