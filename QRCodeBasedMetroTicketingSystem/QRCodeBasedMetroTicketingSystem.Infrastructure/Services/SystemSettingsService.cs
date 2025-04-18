using AutoMapper;
using QRCodeBasedMetroTicketingSystem.Application.Common.Result;
using QRCodeBasedMetroTicketingSystem.Application.DTOs;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Repositories;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services;
using QRCodeBasedMetroTicketingSystem.Domain.Entities;

namespace QRCodeBasedMetroTicketingSystem.Infrastructure.Services
{
    public class SystemSettingsService : ISystemSettingsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;
        private readonly IMapper _mapper;
        private const string CacheKey = "system_settings";

        public SystemSettingsService(IUnitOfWork unitOfWork, ICacheService cacheService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
            _mapper = mapper;
        }

        public async Task<SystemSettingsDto> GetCurrentSettingsAsync()
        {
            // Try to get settings from cache
            var cachedSettings = await _cacheService.GetAsync<SystemSettingsDto>(CacheKey);
            if (cachedSettings != null)
            {
                return cachedSettings;
            }

            // Cache miss - Fetch settings from the database
            var settings = await _unitOfWork.SettingsRepository.GetCurrentSettingsAsync();
            settings ??= new SystemSettings(); // Default settings

            var settingsDto = _mapper.Map<SystemSettingsDto>(settings);
            await _cacheService.SetAsync(CacheKey, settingsDto);

            return settingsDto;
        }

        public async Task<Result> UpdateSettingsAsync(SystemSettingsDto settingsDto)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var settings = await _unitOfWork.SettingsRepository.GetCurrentSettingsAsync();
                if (settings == null)
                {
                    return Result.Failure("An error occurred while updating the settings.");
                }

                settings.MinFare = settingsDto.MinFare;
                settings.FarePerKm = settingsDto.FarePerKm;
                settings.RapidPassQrCodeValidityMinutes = settingsDto.RapidPassQrCodeValidityMinutes;
                settings.QrTicketValidityMinutes = settingsDto.QrTicketValidityMinutes;
                settings.MaxTripDurationMinutes = settingsDto.MaxTripDurationMinutes;
                settings.TimeLimitPenaltyFee = settingsDto.TimeLimitPenaltyFee;

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
                await _cacheService.RemoveAsync(CacheKey);

                return Result.Success("System settings detail updated successfully.");
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                return Result.Failure("An error occurred while updating the settings.");
            }
        }
    }  
}
