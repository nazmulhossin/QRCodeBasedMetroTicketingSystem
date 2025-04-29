using QRCodeBasedMetroTicketingSystem.Application.DTOs;

namespace QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services
{
    public interface ITicketScanService
    {
        Task<ScanTicketResponseDto> ProcessTicketScanAsync(ScanTicketRequest request);
    }
}
