using AutoMapper;
using BL.BLModels;
using BL.DALModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Mapping
{
    public class AutomapperProfile : Profile
    {
        public AutomapperProfile()
        {
            CreateMap<DALModels.Video, BLModels.BLVideo>();

            CreateMap<DALModels.Country, BLModels.BLCountry>();

            CreateMap<DALModels.Tag, BLModels.BLTag>();

            CreateMap<DALModels.Genre, BLModels.BLGenre>();


            CreateMap<Genre, BLGenre>(); 
            CreateMap<BLGenre, Genre>();


            //PublicModule bellow
            CreateMap<DALModels.User, BLModels.BLUser>();
        }
    }
}
