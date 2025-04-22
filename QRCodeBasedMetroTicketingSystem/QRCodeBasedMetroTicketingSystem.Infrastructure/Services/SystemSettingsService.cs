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

        public async Task<SystemSettingsDto> GetSystemSettingsAsync()
        {
            // Try to get settings from cache
            var cachedSettings = await _cacheService.GetAsync<SystemSettingsDto>(CacheKey);
            if (cachedSettings != null)
            {
                return cachedSettings;
            }

            // Cache miss - Fetch settings from the database
            var systemSettings = await _unitOfWork.SystemSettingsRepository.GetSystemSettingsAsync();
            systemSettings ??= new SystemSettings(); // Default settings

            var systemSettingsDto = _mapper.Map<SystemSettingsDto>(systemSettings);
            await _cacheService.SetAsync(CacheKey, systemSettingsDto);

            return systemSettingsDto;
        }

        public async Task<Result> UpdateSettingsAsync(SystemSettingsDto systemSettingsDto)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var systemSettings = await _unitOfWork.SystemSettingsRepository.GetSystemSettingsAsync();
                if (systemSettings == null)
                {
                    return Result.Failure("An error occurred while updating the settings.");
                }

                systemSettings.MinFare = systemSettingsDto.MinFare;
                systemSettings.FarePerKm = systemSettingsDto.FarePerKm;
                systemSettings.RapidPassQrCodeValidityMinutes = systemSettingsDto.RapidPassQrCodeValidityMinutes;
                systemSettings.QrTicketValidityMinutes = systemSettingsDto.QrTicketValidityMinutes;
                systemSettings.MaxTripDurationMinutes = systemSettingsDto.MaxTripDurationMinutes;
                systemSettings.TimeLimitPenaltyFee = systemSettingsDto.TimeLimitPenaltyFee;

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
