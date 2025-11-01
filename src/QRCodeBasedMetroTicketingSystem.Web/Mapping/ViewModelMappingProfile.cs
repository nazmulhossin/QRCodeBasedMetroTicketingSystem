using AutoMapper;
using QRCodeBasedMetroTicketingSystem.Application.DTOs;
using QRCodeBasedMetroTicketingSystem.Web.Areas.Admin.ViewModels;
using QRCodeBasedMetroTicketingSystem.Web.Areas.User.ViewModels;
using QRCodeBasedMetroTicketingSystem.Web.Models;

namespace QRCodeBasedMetroTicketingSystem.Web.Mapping
{
    public class ViewModelMappingProfile : Profile
    {
        public ViewModelMappingProfile()
        {
            CreateMap<AdminDashboardDto, AdminDashboardViewModel>().ReverseMap();
            CreateMap<StationSummaryDto, StationSummaryViewModel>().ReverseMap();
            CreateMap<StationCreationDto, StationCreationViewModel>().ReverseMap();
            CreateMap<StationEditDto, StationEditViewModel>().ReverseMap();
            CreateMap<AdjacentStationDistanceDto, AdjacentStationDistanceViewModel>().ReverseMap();
            CreateMap<StationDeletionDto, StationDeletionViewModel>().ReverseMap();
            CreateMap<SystemSettingsDto, SystemSettingsViewModel>().ReverseMap();
            CreateMap<FareAndDistancesDto, FareAndDistancesViewModel>().ReverseMap();
            CreateMap<RegisterUserDto, RegisterUserViewModel>().ReverseMap();
            CreateMap<UserDto, UserProfileViewModel>().ReverseMap();
            CreateMap<WalletDto, WalletViewModel>().ReverseMap();
            CreateMap<TransactionDto, TransactionViewModel>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
                .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.PaymentMethod.ToString()))
                .ForMember(dest => dest.PaymentFor, opt => opt.MapFrom(src => src.PaymentFor.ToString()))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.CreatedAt));
            CreateMap<TicketDto, TicketViewModel>().ReverseMap();
            CreateMap<TicketDto, PurchaseStatusViewModel>().ReverseMap();
        }
    }
}
