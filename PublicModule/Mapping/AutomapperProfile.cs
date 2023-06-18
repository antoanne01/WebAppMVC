using AutoMapper;
using BL.BLModels;

namespace PublicModule.Mapping
{
    public class AutomapperProfile : Profile
    {
        public AutomapperProfile()
        {
            CreateMap<BLUser, ViewModel.VMUser>();
            CreateMap<BLVideo, ViewModel.ChooseVideoContent>();
            CreateMap<BLImage, ViewModel.ChooseVideoContent>();
        }
    }
}
