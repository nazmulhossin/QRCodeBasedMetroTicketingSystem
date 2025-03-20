using AutoMapper;
using QRCodeBasedMetroTicketingSystem.Application.DTOs;
using QRCodeBasedMetroTicketingSystem.Web.Areas.Admin.ViewModels;

namespace QRCodeBasedMetroTicketingSystem.Web.Mapping
{
    public class ViewModelMappingProfile : Profile
    {
        public ViewModelMappingProfile()
        {
            CreateMap<StationListDto, StationListViewModel>().ReverseMap();
            CreateMap<StationCreationDto, StationCreationViewModel>().ReverseMap();
            CreateMap<StationEditDto, StationEditViewModel>().ReverseMap();
            CreateMap<AdjacentStationDistanceDto, AdjacentStationDistanceViewModel>().ReverseMap();
            CreateMap<StationDeletionDto, StationDeletionViewModel>().ReverseMap();
        }
    }
}
