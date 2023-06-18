using AutoMapper;
using BL.BLModels;
using BL.DALModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Repository
{
    public interface IGenreRepository
    {
        IEnumerable<BLGenre> GetAll();

        Task<BLGenre> AddGenreAsync(BLGenre genre);

        BLGenre GetGenreById(int id);
        void UpdateGenre(BLGenre genre);
        void DeleteGenre(BLGenre genre);

        int GetTotalCount();

        IEnumerable<BLGenre> GetPagedData(int page, int size, string orderBy, string direction);
    }

    public class GenreRepository : IGenreRepository
    {
        private readonly RwaMoviesContext _dbContext;
        private readonly IMapper _mapper;

        public GenreRepository(RwaMoviesContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<BLGenre> AddGenreAsync(BLGenre genre)
        {
            var dbGenre = _mapper.Map<Genre>(genre);

            _dbContext.Genres.Add(dbGenre);
            await _dbContext.SaveChangesAsync();

            return _mapper.Map<BLGenre>(dbGenre);
        }

        public IEnumerable<BLGenre> GetAll()
        {
            var dbGenres = _dbContext.Genres;
            var blGenres = _mapper.Map<IEnumerable<BLGenre>>(dbGenres);
            return blGenres;
        }

        public BLGenre GetGenreById(int id)
        {
            var dbGenre = _dbContext.Genres.Find(id);

            if (dbGenre == null)
            {
                return null;
            }

            var blGenre = _mapper.Map<BLGenre>(dbGenre);
            return blGenre;
        }

        public void UpdateGenre(BLGenre genre)
        {
            var dbGenre = _dbContext.Genres.FirstOrDefault(v => v.Id == genre.Id);

            if (dbGenre == null)
            {
                throw new ArgumentException($"Video with ID {dbGenre.Id} not found.");
            }

            // Update the properties of the dbVideo entity with the values from the BLVideo object
            dbGenre.Name = genre.Name;
            dbGenre.Description = genre.Description;

            _dbContext.SaveChanges();
        }

        public void DeleteGenre(BLGenre genre)
        {
            using (var dbTran = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var dbGenre = _dbContext.Genres.FirstOrDefault(x => x.Id == genre.Id);
                    if (dbGenre == null)
                    {
                        throw new InvalidOperationException("Genre not found");
                    }
                    var dbVideos = _dbContext.Videos.Where(x => x.GenreId == genre.Id);

                    _dbContext.Videos.RemoveRange(dbVideos);
                    _dbContext.SaveChanges();

                    _dbContext.Genres.Remove(dbGenre);

                    _dbContext.SaveChanges();

                    dbTran.Commit();
                }
                catch (Exception)
                {
                    dbTran.Rollback();
                    throw;
                }
            }
        }

        public int GetTotalCount()
        {
            return _dbContext.Genres.Count();
        }

        public IEnumerable<BLGenre> GetPagedData(int page, int size, string orderBy, string direction)
        {
            IEnumerable<Genre> dbGenres = _dbContext.Genres.AsEnumerable();

            // Ordering
            if (string.Compare(orderBy, "id", true) == 0)
            {
                dbGenres = dbGenres.OrderBy(x => x.Id);
            }
            else if (string.Compare(orderBy, "name", true) == 0)
            {
                dbGenres = dbGenres.OrderBy(x => x.Name);
            }
            else if (string.Compare(orderBy, "description", true) == 0)
            {
                dbGenres = dbGenres.OrderBy(x => x.Description);
            }
            else
            {
                // default: order by Id
                dbGenres = dbGenres.OrderBy(x => x.Id);
            }

            // For descending order we just reverse it
            if (string.Compare(direction, "desc", true) == 0)
            {
                dbGenres = dbGenres.Reverse();
            }

            // Now we can page the correctly ordered items
            dbGenres = dbGenres.Skip(page * size).Take(size);

            var blGenres = _mapper.Map<IEnumerable<BLGenre>>(dbGenres);

            return blGenres;
        }
    }
}