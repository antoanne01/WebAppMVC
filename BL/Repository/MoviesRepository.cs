using AutoMapper;
using BL.BLModels;
using BL.DALModels;

namespace BL.Repository
{
    public interface IMoviesRepository
    {
        IEnumerable<BLVideo> GetAll();
        BLVideo BLAddVideo(string name, string description, int genreId, int totalSeconds, string streamingUrl, int imageid);
        Dictionary<string, int> GetGenreMappings();
        Dictionary<int, string> GetGenres();

        BLVideo GetVideoById(int id);
        void UpdateVideo(BLVideo video);
        void DeleteVideo(BLVideo video);

        int GetTotalCount();

        IEnumerable<BLVideo> GetPagedData(int page, int size, string orderBy, string direction);
    }

    public class VideoRepository : IMoviesRepository
    {
        private readonly RwaMoviesContext _dbContext;
        private readonly IMapper _mapper;

        public VideoRepository(RwaMoviesContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public IEnumerable<BLVideo> GetAll()
        {
            var dbVideos = _dbContext.Videos;

            var blVideos = _mapper.Map<IEnumerable<BLVideo>>(dbVideos);

            return blVideos;
        }

        public BLVideo BLAddVideo(string name, string description, int genreId, int totalSeconds, string streamingUrl, int Imageid)
        {
            var dbVideo = new Video()
            {
                CreatedAt = DateTime.UtcNow,
                Name = name,
                Description = description,
                GenreId = genreId,
                TotalSeconds = totalSeconds,
                StreamingUrl = streamingUrl,
                ImageId = Imageid
            };

            _dbContext.Videos.Add(dbVideo);
            _dbContext.SaveChanges();

            var blVideo = _mapper.Map<BLVideo>(dbVideo);
            return blVideo;
        }

        public Dictionary<string, int> GetGenreMappings()
        {
            var genreMappings = new Dictionary<string, int>();

            // Retrieve genre mappings from your data source (e.g., database)
            var genres = _dbContext.Genres.ToList();

            // Populate the genreMappings dictionary
            foreach (var genre in genres)
            {
                genreMappings.Add(genre.Name, genre.Id);
            }

            return genreMappings;
        }

        public Dictionary<int, string> GetGenres()
        {
            var genreMappings = new Dictionary<int, string>();

            var genres = _dbContext.Genres.ToList();

            foreach (var genre in genres)
            {
                genreMappings.Add(genre.Id, genre.Name);
            }

            return genreMappings;
        }

        public BLVideo GetVideoById(int id)
        {
            var dbVideo = _dbContext.Videos.Find(id);

            if (dbVideo == null)
            {
                return null;
            }

            var blVideo = _mapper.Map<BLVideo>(dbVideo);
            return blVideo;
        }

        public void UpdateVideo(BLVideo video)
        {
            var dbVideo = _dbContext.Videos.FirstOrDefault(v => v.Id == video.Id);

            if (dbVideo == null)
            {
                throw new ArgumentException($"Video with ID {video.Id} not found.");
            }

            // Update the properties of the dbVideo entity with the values from the BLVideo object
            dbVideo.Name = video.Name;
            dbVideo.Description = video.Description;
            dbVideo.GenreId = video.GenreId;
            dbVideo.TotalSeconds = video.TotalSeconds;
            dbVideo.StreamingUrl = video.StreamingUrl;

            _dbContext.SaveChanges();
        }

        public void DeleteVideo(BLVideo video)
        {
            var dbVideo = _dbContext.Videos.FirstOrDefault(d => d.Id == video.Id);

            if (dbVideo == null)
            {
                throw new ArgumentException($"Video with ID {video.Id} not found.");
            }

            _dbContext.Videos.Remove(dbVideo);
            _dbContext.SaveChanges();
        }

        public int GetTotalCount()
        {
            return _dbContext.Videos.Count();
        }

        public IEnumerable<BLVideo> GetPagedData(int page, int size, string orderBy, string direction)
        {
            IEnumerable<Video> dbVideo = _dbContext.Videos.AsEnumerable();
            // Ordering
            if (string.Compare(orderBy, "id", true) == 0)
            {
                dbVideo = dbVideo.OrderBy(x => x.Id);
            }
            else if (string.Compare(orderBy, "name", true) == 0)
            {
                dbVideo = dbVideo.OrderBy(x => x.Name);
            }
            else if (string.Compare(orderBy, "description", true) == 0)
            {
                dbVideo = dbVideo.OrderBy(x => x.Description);
            }
            else if (string.Compare(orderBy, "genreId", true) == 0)
            {
                dbVideo = dbVideo.OrderBy(x => x.GenreId);
            }
            else if (string.Compare(orderBy, "totalSeconds", true) == 0)
            {
                dbVideo = dbVideo.OrderBy(x => x.TotalSeconds);
            }
            else
            {
                // default: order by Id
                dbVideo = dbVideo.OrderBy(x => x.Id);
            }

            // For descending order we just reverse it
            if (string.Compare(direction, "desc", true) == 0)
            {
                dbVideo = dbVideo.Reverse();
            }

            // Now we can page the correctly ordered items
            dbVideo = dbVideo.Skip(page * size).Take(size);

            var blVideo = _mapper.Map<IEnumerable<BLVideo>>(dbVideo);

            return blVideo;
        }
    }
}
