using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using QRCodeBasedMetroTicketingSystem.Application.DTOs;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services;
using QRCodeBasedMetroTicketingSystem.Domain.Entities;
using QRCodeBasedMetroTicketingSystem.Web.Areas.Admin.ViewModels;

namespace QRCodeBasedMetroTicketingSystem.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SystemSettingsController : Controller
    {
        private readonly ISettingsService _settingsService;
        private readonly IMapper _mapper;

        public SystemSettingsController(ISettingsService settingsService, IMapper mapper)
        {
            _settingsService = settingsService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var settingsDto = await _settingsService.GetCurrentSettingsAsync();
            var viewModel = _mapper.Map<SettingsViewModel>(settingsDto);
            return View(viewModel);
        }

        public async Task<IActionResult> Edit()
        {
            var settingsDto = await _settingsService.GetCurrentSettingsAsync();
            var viewModel = _mapper.Map<SettingsViewModel>(settingsDto);
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SettingsViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            // Validate business rules
            if (viewModel.MaxFare < viewModel.MinFare)
            {
                ModelState.AddModelError("MaxFare", "Maximum fare must be greater than or equal to minimum fare");
                return View(viewModel);
            }

            var settingsDto = _mapper.Map<SettingsDto>(viewModel);
            var result = await _settingsService.UpdateSettingsAsync(settingsDto);

            if (result.IsSuccess)
                TempData["SuccessMessage"] = result.Message;
            else
                TempData["ErrorMessage"] = result.Message;

            return RedirectToAction("Index");
        }
    }
}
