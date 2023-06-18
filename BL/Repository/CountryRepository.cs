using AutoMapper;
using BL.BLModels;
using BL.DALModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Repository
{
    public interface ICountryRepository
    {
        IEnumerable<BLCountry> GetAll();

        BLCountry GetCountryById(int id);

        BLCountry AddCountry(string code, string name);
        void UpdateCountry(BLCountry country);
        void DeleteCountry(BLCountry country);

        int GetTotalCount();

        IEnumerable<BLCountry> GetPagedData(int page, int size, string orderBy, string direction);
    }

    public class CountryRepository : ICountryRepository
    {
        private readonly RwaMoviesContext _dbContext;
        private readonly IMapper _mapper;

        public CountryRepository(RwaMoviesContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public IEnumerable<BLCountry> GetAll()
        {
            var dbCountries = _dbContext.Country;
            var blCountries = _mapper.Map<IEnumerable<BLCountry>>(dbCountries);
            return blCountries;
        }

        public BLCountry GetCountryById(int id)
        {
            var dbCountry = _dbContext.Country.Find(id);

            if (dbCountry == null)
            {
                return null;
            }

            var blCountry = _mapper.Map<BLCountry>(dbCountry);
            return blCountry;
        }

        public BLCountry AddCountry(string code, string name)
        {
            var dbCountry = new Country()
            {
                Code = code,
                Name = name
            };

            _dbContext.Country.Add(dbCountry);
            _dbContext.SaveChanges();

            var blCountry = _mapper.Map<BLCountry>(dbCountry);
            return blCountry;
        }

        public void UpdateCountry(BLCountry country)
        {
            var dbCountry = _dbContext.Country.FirstOrDefault(v => v.Id == country.Id);

            if (dbCountry == null)
            {
                throw new ArgumentException($"Cannot find requested video");
            }

            // Update the properties of the dbVideo entity with the values from the BLVideo object
            dbCountry.Code = country.Code;
            dbCountry.Name = country.Name;

            _dbContext.SaveChanges();
        }

        public void DeleteCountry(BLCountry country)
        {
            using (var dbTran = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var dbCountry = _dbContext.Country.FirstOrDefault(x => x.Id == country.Id);
                    if (dbCountry == null)
                    {
                        throw new InvalidOperationException("Country not found");
                    }
                    var dbUsers = _dbContext.Users.Where(x => x.CountryOfResidenceId == dbCountry.Id);

                    _dbContext.Users.RemoveRange(dbUsers);
                    _dbContext.SaveChanges();

                    _dbContext.Country.Remove(dbCountry);

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
            return _dbContext.Country.Count();
        }

        public IEnumerable<BLCountry> GetPagedData(int page, int size, string orderBy, string direction)
        {
            IEnumerable<Country> dbCountry = _dbContext.Country.AsEnumerable();

            // Ordering
            if (string.Compare(orderBy, "id", true) == 0)
            {
                dbCountry = dbCountry.OrderBy(x => x.Id);
            }
            else if (string.Compare(orderBy, "code", true) == 0)
            {
                dbCountry = dbCountry.OrderBy(x => x.Code);
            }
            else if (string.Compare(orderBy, "name", true) == 0)
            {
                dbCountry = dbCountry.OrderBy(x => x.Name);
            }
            else
            {
                // default: order by Id
                dbCountry = dbCountry.OrderBy(x => x.Id);
            }

            // For descending order we just reverse it
            if (string.Compare(direction, "desc", true) == 0)
            {
                dbCountry = dbCountry.Reverse();
            }

            // Now we can page the correctly ordered items
            dbCountry = dbCountry.Skip(page * size).Take(size);

            var blCountry = _mapper.Map<IEnumerable<BLCountry>>(dbCountry);

            return blCountry;
        }
    }
}
