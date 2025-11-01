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
            CreateMap<Ticket, TicketDto>()
                .ForMember(dest => dest.OriginStationName, opt => opt.MapFrom(src => src.OriginStation != null ? src.OriginStation.Name : null))
                .ForMember(dest => dest.DestinationStationName, opt => opt.MapFrom(src => src.DestinationStation != null ? src.DestinationStation.Name : null));
            CreateMap<Trip, TripDto>()
                .ForMember(dest => dest.EntryStationName, opt => opt.MapFrom(src => src.EntryStation != null ? src.EntryStation.Name : null))
                .ForMember(dest => dest.ExitStationName, opt => opt.MapFrom(src => src.ExitStation != null ? src.ExitStation.Name : null));
        }
    }
}
