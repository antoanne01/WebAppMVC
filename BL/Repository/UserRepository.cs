using AutoMapper;
using BL.BLModels;
using BL.DALModels;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BL.Repository
{
    public interface IUserRepository
    {
        IEnumerable<BLUser> GetAll();
        BLUser CreateUser(string username, string firstName, string lastName, string email, string password, string phone, int country);
        IEnumerable<BLCountry> GetCountries();
        Dictionary<int, string> GetCountry();
        BLUser GetConfirmedUser(string username, string password);

        BLUser GetUserByEmail(string username);
        BLUser GetUserByUsername(string username);

        void ChangePassword(string username, string newPassword);

        //Bellow methods - ConfirmedUserController

        IEnumerable<BLUser> GetAllConfirmed();
        BLUser GetUserById(int id);
        BLCountry GetCountryById(int countryId);

        void UpdateConfirmedUser(BLUser user);

        int GetTotalCount();

        void DeleteUser(BLUser user);

        IEnumerable<BLVideo> GetPagedData(int page, int size, string orderBy, string direction);
    }



    public class UserRepository : IUserRepository
    {
        private readonly RwaMoviesContext _dbContext;
        private readonly IMapper _mapper;

        public UserRepository(RwaMoviesContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public IEnumerable<BLUser> GetAll()
        {
            var dbUsers = _dbContext.Users;

            var blUsers = _mapper.Map<IEnumerable<BLUser>>(dbUsers);

            return blUsers;
        }

        public BLUser CreateUser(string username, string firstName, string lastName, string email, string password, string phone, int country)
        {
            try
            {
                if (_dbContext.Users.Any(u => u.Email == email))
                {
                    throw new InvalidOperationException("Register not allowed, try with different mail");
                }

                (var salt, var b64Salt) = GenerateSalt();
                var b64Hash = CreateHash(password, salt);
                var b64SecToken = GenerateSecurityToken();

                // Create BLUser object
                var dbUser = new User()
                {
                    CreatedAt = DateTime.UtcNow,
                    Username = username,
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    PwdHash = (string)b64Hash,
                    PwdSalt = b64Salt,
                    IsConfirmed = true,
                    Phone = phone,
                    SecurityToken = (string)b64SecToken,
                    CountryOfResidenceId = country
                };

                _dbContext.Users.Add(dbUser);

                _dbContext.SaveChanges();

                var blUser = _mapper.Map<BLUser>(dbUser);
                return blUser;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            
        }

        private object GenerateSecurityToken()
        {
            byte[] securityToken = RandomNumberGenerator.GetBytes(256 / 8);
            string b64SecToken = Convert.ToBase64String(securityToken);

            return b64SecToken;
        }

        private string CreateHash(string password, byte[] salt)
        {
            byte[] hash =
                KeyDerivation.Pbkdf2(
                    password: password,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100000,
                    numBytesRequested: 256 / 8);
            string b64Hash = Convert.ToBase64String(hash);

            return b64Hash;
        }

        private static (byte[], string) GenerateSalt()
        {
            // Generate salt
            var salt = RandomNumberGenerator.GetBytes(128 / 8);
            var b64Salt = Convert.ToBase64String(salt);

            return (salt, b64Salt);
        }

        public IEnumerable<BLCountry> GetCountries()
        {
            var dbCountries = _dbContext.Country.ToList();
            var blCountries = _mapper.Map<IEnumerable<BLCountry>>(dbCountries);
            return blCountries;
        }

        public BLUser GetConfirmedUser(string username, string password)
        {
            var dbUser = _dbContext.Users.FirstOrDefault(x =>
                x.Username == username &&
                x.IsConfirmed == true);

            if (dbUser == null)
                throw new InvalidOperationException("Wrong username or password");

            var salt = Convert.FromBase64String(dbUser.PwdSalt);
            var b64Hash = CreateHash(password, salt);

            if (dbUser.PwdHash.Trim() != b64Hash.ToString().Trim())
                throw new InvalidOperationException("Wrong username or password");

            var blUser = _mapper.Map<BLUser>(dbUser);

            return blUser;

        }

        public BLUser GetUserByEmail(string email)
        {
            var userProfile = _dbContext.Users.FirstOrDefault(u => u.Email == email);

            var blUser = _mapper.Map<BLUser>(userProfile);

            return blUser;
        }
        
        public BLUser GetUserByUsername(string username)
        {
            var userProfile = _dbContext.Users.FirstOrDefault(u => u.Username == username);

            var blUser = _mapper.Map<BLUser>(userProfile);

            return blUser;
        }

        public void ChangePassword(string username, string newPassword)
        {
            var userToChangePassword = _dbContext.Users.FirstOrDefault(x =>
                x.Username == username &&
                x.DeletedAt == null);

            if (userToChangePassword == null)
            {
                throw new InvalidOperationException("User not found.");
            }

            (var salt, var b64Salt) = GenerateSalt();

            var b64Hash = CreateHash(newPassword, salt);

            userToChangePassword.PwdHash = b64Hash;
            userToChangePassword.PwdSalt = b64Salt;

            _dbContext.SaveChanges();
        }

        //Bellow methods - ConfirmedUserController

        public IEnumerable<BLUser> GetAllConfirmed()
        {

            var dbUsers = _dbContext.Users.Where(d => d.IsConfirmed == true).ToList();

            var blUsers = _mapper.Map<IEnumerable<BLUser>>(dbUsers);

            var countryIds = dbUsers.Select(u => u.CountryOfResidenceId).Distinct().ToList();
            var countries = _dbContext.Country.Where(c => countryIds.Contains(c.Id)).ToList();

            foreach (var user in blUsers)
            {
                var country = countries.FirstOrDefault(c => c.Id == user.CountryOfResidenceId);
                if (country != null)
                {
                    user.CountryName = country.Name;
                }
            }

            return blUsers;
        }

        public BLUser GetUserById(int id)
        {
            var dbUser = _dbContext.Users.Find(id);

            if (dbUser == null)
            {
                return null;
            }

            var blUser = _mapper.Map<BLUser>(dbUser);
            return blUser;
        }

        public BLCountry GetCountryById(int countryId)
        {
            var dbCountry = _dbContext.Country.FirstOrDefault(c => c.Id == countryId);
            var blCountry = _mapper.Map<BLCountry>(dbCountry);
            return blCountry;
        }

        public void UpdateConfirmedUser(BLUser user)
        {
            var dbUser = _dbContext.Users.FirstOrDefault(v => v.Id == user.Id);

            if (dbUser == null)
            {
                throw new ArgumentException($"Video with ID {dbUser.Id} not found.");
            }

            dbUser.Username = user.Username;
            dbUser.FirstName = user.FirstName;
            dbUser.LastName = user.LastName;
            dbUser.Email = user.Email;
            dbUser.Phone = user.Phone;
            dbUser.IsConfirmed = true;
            dbUser.CountryOfResidenceId = user.CountryOfResidenceId;

            _dbContext.SaveChanges();
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

        public int GetTotalCount()
        {
            return _dbContext.Videos.Count();
        }

        public Dictionary<int, string> GetCountry()
        {
            var countryMappings = new Dictionary<int, string>();

            var country = _dbContext.Country.ToList();

            foreach (var c in country)
            {
                countryMappings.Add(c.Id, c.Name);
            }

            return countryMappings;
        }

        public void DeleteUser(BLUser user)
        {
            var dbUser = _dbContext.Users.FirstOrDefault(u => u.Id == user.Id);

            if (dbUser == null)
            {
                throw new ArgumentException($"User not found.");
            }

            _dbContext.Users.Remove(dbUser);
            _dbContext.SaveChanges();
        }
    }
}
