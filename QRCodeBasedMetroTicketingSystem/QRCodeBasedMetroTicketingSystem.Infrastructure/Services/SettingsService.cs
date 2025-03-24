using AutoMapper;
using QRCodeBasedMetroTicketingSystem.Application.Common.Result;
using QRCodeBasedMetroTicketingSystem.Application.DTOs;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Repositories;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services;
using QRCodeBasedMetroTicketingSystem.Domain.Entities;

namespace QRCodeBasedMetroTicketingSystem.Infrastructure.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SettingsService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<SettingsDto> GetCurrentSettingsAsync()
        {
            var settings = await _unitOfWork.SettingsRepository.GetCurrentSettingsAsync();

            if (settings == null)
            {
                settings = new Settings(); // Default settings
            }

            return _mapper.Map<SettingsDto>(settings);
        }

        public async Task<Result> UpdateSettingsAsync(SettingsDto settingsDto)
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
                settings.MaxFare = settingsDto.MaxFare;
                settings.FarePerKm = settingsDto.FarePerKm;
                settings.QrCodeValidTime = settingsDto.QrCodeValidTime;

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return Result.Success("System settings detail updated successfully.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return Result.Failure($"An error occurred while updating the settings: {ex.Message}");
            }
        }
    }  
}
