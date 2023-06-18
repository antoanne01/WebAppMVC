using AutoMapper;

namespace AdminModule.Mapping
{
    public class AutomapperProfile : Profile
    {
        public AutomapperProfile()
        {
            CreateMap<BL.BLModels.BLVideo, ViewModel.VMVideo>();
            CreateMap<BL.BLModels.BLVideo, ViewModel.VMEditVideo>();
            CreateMap<BL.BLModels.BLVideo, ViewModel.VMDeleteVideo>();

            CreateMap<BL.BLModels.BLCountry, ViewModel.VMCountries>();

            CreateMap<BL.BLModels.BLTag, ViewModel.VMTag>();

            CreateMap<BL.BLModels.BLGenre, ViewModel.VMGenre>();
            CreateMap<BL.BLModels.BLGenre, ViewModel.VMEditGenre>();

            CreateMap<BL.BLModels.BLUser, ViewModel.VMUser>();
        }
    }
}
