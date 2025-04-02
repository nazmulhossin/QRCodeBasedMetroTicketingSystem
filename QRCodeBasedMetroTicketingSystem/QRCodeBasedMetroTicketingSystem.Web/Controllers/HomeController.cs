using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using QRCodeBasedMetroTicketingSystem.Application.DTOs;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services;
using QRCodeBasedMetroTicketingSystem.Web.Models;

namespace QRCodeBasedMetroTicketingSystem.Web.Controllers;

[Route("")]
public class HomeController : Controller
{
    private readonly IFareCalculationService _fareCalculationService;
    private readonly IStationService _stationService;
    private readonly IMapper _mapper;

    public HomeController(IStationService stationService, IMapper mapper, IFareCalculationService fareCalculationService)
    {
        _stationService = stationService;
        _mapper = mapper;
        _fareCalculationService = fareCalculationService;
    }

    public IActionResult Root()
    {
        return RedirectToAction("Index", "Home");
    }

    [Route("Home")]
    public async Task<IActionResult> Index()
    {
        var stationList = await _stationService.GetAllStationsOrderedAsync();
        var viewModel = new HomeViewModel { StationList = stationList };
        return View(viewModel);
    }

    [HttpPost("GetFare")]
    public async Task<IActionResult> CalculateFare([FromBody] FareCalculationRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (request.FromStationId == request.ToStationId)
        {
            return BadRequest(new { message = "Departure and destination stations cannot be the same." });
        }

        try
        {
            var fareDistanceList = await _fareCalculationService.GetFareDistancesAsync(request.FromStationId, request.ToStationId);
            var result = fareDistanceList.FirstOrDefault();
            return Ok(new FareDistanceDto
            {
                FromStationName = result.FromStationName,
                ToStationName = result.ToStationName,
                Distance = result.Distance,
                Fare = result.Fare
            });
        }
        catch
        {
            return StatusCode(500, new { message = "An error occurred while calculating the fare." });
        }
    }

    [Route("MapAndRoutes")]
    public IActionResult MapAndRoutes()
    {
        return View();
    }
}
