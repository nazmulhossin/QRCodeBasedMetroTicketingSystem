using QRCodeBasedMetroTicketingSystem.Application.Common.Result;
using QRCodeBasedMetroTicketingSystem.Application.DTOs;

namespace QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services
{
    public interface ISystemSettingsService
    {
        Task<SettingsDto> GetCurrentSettingsAsync();
        Task<Result> UpdateSettingsAsync(SettingsDto settingsDto);
    }
}
