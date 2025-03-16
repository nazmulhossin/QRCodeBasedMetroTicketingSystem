using AutoMapper;
using QRCodeBasedMetroTicketingSystem.Application.DTOs;
using QRCodeBasedMetroTicketingSystem.Web.Areas.Admin.ViewModels;

namespace QRCodeBasedMetroTicketingSystem.Web.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<StationListDto, StationListViewModel>().ReverseMap();
            CreateMap<StationCreationDto, StationCreationViewModel>().ReverseMap();
        }
    }
}
