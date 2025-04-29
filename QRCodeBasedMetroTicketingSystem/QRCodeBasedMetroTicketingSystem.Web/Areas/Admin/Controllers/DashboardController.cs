using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services;
using QRCodeBasedMetroTicketingSystem.Web.Areas.Admin.ViewModels;
using QRCodeBasedMetroTicketingSystem.Web.Models;

namespace QRCodeBasedMetroTicketingSystem.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(AuthenticationSchemes = AuthSchemes.AdminScheme, Roles = ApplicationRoles.Admin)]
    public class DashboardController : Controller
    {
        private readonly IAdminDashboardService _adminDashboardService;
        private readonly IMapper _mapper;

        public DashboardController(IAdminDashboardService adminDashboardService, IMapper mapper)
        {
            _adminDashboardService = adminDashboardService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var dashboardDataDto = await _adminDashboardService.GetDashboardDataAsync();
            var viewModel = _mapper.Map<AdminDashboardViewModel>(dashboardDataDto);
            return View(viewModel);
        }
    }
}
