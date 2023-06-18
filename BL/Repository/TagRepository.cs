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
    public interface ITagRepository
    {
        BLTag BLAddTag(string name);
        IEnumerable<BLTag> GetAll();
        BLTag GetTagById(int id);
        void UpdateTag(BLTag tag);
        void DeleteTag(BLTag tag);

        int GetTotalCount();

        IEnumerable<BLTag> GetPagedData(int page, int size, string orderBy, string direction);
    }

    public class TagRepository : ITagRepository
    {
        private readonly RwaMoviesContext _dbContext;
        private readonly IMapper _mapper;

        public TagRepository(RwaMoviesContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public BLTag BLAddTag(string name)
        {
            var dbTag = new Tag()
            {
                Name = name
            };

            _dbContext.Tags.Add(dbTag);
            _dbContext.SaveChanges();

            var bltag = _mapper.Map<BLTag>(dbTag);
            return bltag;
        }

        public IEnumerable<BLTag> GetAll()
        {
            var dbTags = _dbContext.Tags;
            var blTags = _mapper.Map<IEnumerable<BLTag>>(dbTags);
            return blTags;
        }

        public BLTag GetTagById(int id)
        {
            var dbTag = _dbContext.Tags.Find(id);

            if (dbTag == null)
            {
                return null;
            }

            var blTag = _mapper.Map<BLTag>(dbTag);
            return blTag;
        }

        public void UpdateTag(BLTag tag)
        {
            var dbTag = _dbContext.Tags.FirstOrDefault(v => v.Id == tag.Id);

            if (dbTag == null)
            {
                throw new ArgumentException($"Video with ID {tag.Id} not found.");
            }

            // Update the properties of the dbVideo entity with the values from the BLVideo object
            dbTag.Name = tag.Name;

            _dbContext.SaveChanges();
        }

        public int GetTotalCount()
        {
            return _dbContext.Tags.Count();
        }

        public void DeleteTag(BLTag tag)
        {
            var dbTag = _dbContext.Tags.FirstOrDefault(d => d.Id == tag.Id);

            if (dbTag == null)
            {
                throw new ArgumentException($"Video with ID {tag.Id} not found.");
            }

            _dbContext.Tags.Remove(dbTag);
            _dbContext.SaveChanges();
        }

        public IEnumerable<BLTag> GetPagedData(int page, int size, string orderBy, string direction)
        {
            IEnumerable<Tag> dbTags = _dbContext.Tags.AsEnumerable();

            // Ordering
            if (string.Compare(orderBy, "id", true) == 0)
            {
                dbTags = dbTags.OrderBy(x => x.Id);
            }
            else if (string.Compare(orderBy, "name", true) == 0)
            {
                dbTags = dbTags.OrderBy(x => x.Name);
            }
            
            else
            {
                // default: order by Id
                dbTags = dbTags.OrderBy(x => x.Id);
            }

            // For descending order we just reverse it
            if (string.Compare(direction, "desc", true) == 0)
            {
                dbTags = dbTags.Reverse();
            }

            // Now we can page the correctly ordered items
            dbTags = dbTags.Skip(page * size).Take(size);

            var blTags = _mapper.Map<IEnumerable<BLTag>>(dbTags);

            return blTags;
        }
    }
}
