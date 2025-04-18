using AutoMapper;
using QRCodeBasedMetroTicketingSystem.Application.DTOs;
using QRCodeBasedMetroTicketingSystem.Domain.Entities;

namespace QRCodeBasedMetroTicketingSystem.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<StationSummaryDto, Station>().ReverseMap();
            CreateMap<StationDistanceDto, StationDistance>().ReverseMap();
            CreateMap<SystemSettingsDto, SystemSettings>().ReverseMap();
            CreateMap<RegisterUserDto, User>().ReverseMap();
            CreateMap<UserDto, User>().ReverseMap();
            CreateMap<WalletDto, Wallet>().ReverseMap();
            CreateMap<TransactionDto, Transaction>().ReverseMap();
        }
    }
}
