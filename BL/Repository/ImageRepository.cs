using AutoMapper;
using BL.BLModels;
using BL.DALModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Repository
{

    public interface IImageRepository
    {
        int CreateImage(IFormFile ImageUpload);
        BLImage GetImage(int id);
    };

    public class ImageRepository : IImageRepository
    {
        private readonly RwaMoviesContext _dbContext;
        private readonly IMapper _mapper;

        public ImageRepository(RwaMoviesContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public int CreateImage(IFormFile ImageUpload)
        {
            try
            {
                var imageArray = GetFileByte(ImageUpload);

                if (imageArray != null)
                {
                    _dbContext.Images.Add(new Image(){

                        Content = Convert.ToBase64String(imageArray),
                    });
                }
                _dbContext.SaveChanges();
                return _dbContext.Images.OrderBy(image => image.Id).LastOrDefault().Id;

            }
            catch (Exception ex)
            {
                throw new Exception("Error while creating the genre.", ex);
            }
        }

        public BLImage GetImage(int id)
        {
            var dbImage = _dbContext.Images.FirstOrDefault(x => x.Id == id);

            if (dbImage == null)
            {
                return null;
            }

            var blImage = _mapper.Map<BLImage>(dbImage);

            return blImage;
        }



        private static byte[] GetFileByte(IFormFile image)
        {
            if (image != null)
            {
                if (image.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        image.CopyTo(memoryStream);

                        if (memoryStream.Length < 50 * 1024 * 1024)
                        {
                            return memoryStream.ToArray();
                        }
                    }

                }
            }

            return null;
        }
    }
}
