using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QRCodeBasedMetroTicketingSystem.Application.DTOs;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services;
using QRCodeBasedMetroTicketingSystem.Web.Areas.Admin.ViewModels;
using QRCodeBasedMetroTicketingSystem.Web.Models;

namespace QRCodeBasedMetroTicketingSystem.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(AuthenticationSchemes = AuthSchemes.AdminScheme, Roles = ApplicationRoles.Admin)]
    public class SystemSettingsController : Controller
    {
        private readonly ISystemSettingsService _settingsService;
        private readonly IMapper _mapper;

        public SystemSettingsController(ISystemSettingsService settingsService, IMapper mapper)
        {
            _settingsService = settingsService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var settingsDto = await _settingsService.GetSystemSettingsAsync();
            var viewModel = _mapper.Map<SystemSettingsViewModel>(settingsDto);
            return View(viewModel);
        }

        public async Task<IActionResult> Edit()
        {
            var settingsDto = await _settingsService.GetSystemSettingsAsync();
            var viewModel = _mapper.Map<SystemSettingsViewModel>(settingsDto);
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SystemSettingsViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var settingsDto = _mapper.Map<SystemSettingsDto>(viewModel);
            var result = await _settingsService.UpdateSettingsAsync(settingsDto);

            if (result.IsSuccess)
                TempData["SuccessMessage"] = result.Message;
            else
                TempData["ErrorMessage"] = result.Message;

            return RedirectToAction("Index");
        }
    }
}
