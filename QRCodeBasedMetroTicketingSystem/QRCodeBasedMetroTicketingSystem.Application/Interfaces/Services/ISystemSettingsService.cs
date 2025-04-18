using QRCodeBasedMetroTicketingSystem.Application.Common.Result;
using QRCodeBasedMetroTicketingSystem.Application.DTOs;

namespace QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services
{
    public interface ISystemSettingsService
    {
        Task<SystemSettingsDto> GetCurrentSettingsAsync();
        Task<Result> UpdateSettingsAsync(SystemSettingsDto settingsDto);
    }
}
