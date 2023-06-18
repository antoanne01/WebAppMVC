using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Tokens;
using RWA_IntegrationPart.Model;
using RWA_IntegrationPart.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace RWA_IntegrationPart.Services
{
    public class UserRepository : IUserRepository
    {
        private readonly IConfiguration _configuration;
        private readonly RwaMoviesContext _dbContext;

        public UserRepository(IConfiguration configuration, RwaMoviesContext dbContext)
        {
            _configuration = configuration;
            _dbContext = dbContext;
        }

        public User Add(UserRegisterRequest request)
        {
            // Username: Normalize and check if username exists
            var normalizedUsername = request.Username.ToLower().Trim();
            if (_dbContext.Users.Any(x => x.Username.Equals(normalizedUsername)))
                throw new InvalidOperationException("Username already exists");

            // Password: Salt and hash password
            byte[] salt = RandomNumberGenerator.GetBytes(128 / 8); // divide by 8 to convert bits to bytes
            string b64Salt = Convert.ToBase64String(salt);

            byte[] hash =
                KeyDerivation.Pbkdf2(
                    password: request.Password,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100000,
                    numBytesRequested: 256 / 8);
            string b64Hash = Convert.ToBase64String(hash);

            // SecurityToken: Random security token
            byte[] securityToken = RandomNumberGenerator.GetBytes(256 / 8);
            string b64SecToken = Convert.ToBase64String(securityToken);


            // New user
            var newUser = new User
            {
                Username = request.Username,
                CreatedAt = DateTime.UtcNow,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Phone = request.Phone,
                CountryOfResidenceId = request.CountryOfResidenceId,
                IsConfirmed = false,
                SecurityToken = b64SecToken,
                PwdSalt = b64Salt,
                PwdHash = b64Hash,
            };
            _dbContext.Users.Add(newUser);
            CreateNotification(newUser);
            _dbContext.SaveChanges();

            return newUser;
        }

        private void CreateNotification(User newUser)
        {
            var notification = new Notification()
            {
                CreatedAt = DateTime.UtcNow,
                ReceiverEmail = newUser.Email,
                Subject = "Confirm your email adress",
                Body = newUser.SecurityToken,
                SentAt = null
            };
            _dbContext.Notifications.Add(notification);
            _dbContext.SaveChanges();
        }

        public Tokens JwtTokens(JwtTokensRequest request)
        {
            var isAuthenticated = Authenticate(request.Username, request.Password);

            if (!isAuthenticated)
                throw new InvalidOperationException("Authentication failed");

            // Get secret key bytes
            var jwtKey = _configuration["JWT:Key"];
            var jwtKeyBytes = Encoding.UTF8.GetBytes(jwtKey);

            // Create a token descriptor (represents a token, kind of a "template" for token)
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new System.Security.Claims.Claim[]
                {
                    new System.Security.Claims.Claim(ClaimTypes.Name, request.Username),
                    new System.Security.Claims.Claim(JwtRegisteredClaimNames.Sub, request.Username)
                }),
                Issuer = _configuration["JWT:Issuer"],
                Audience = _configuration["JWT:Audience"],
                Expires = DateTime.UtcNow.AddMinutes(10),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(jwtKeyBytes),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            // Create token using that descriptor, serialize it and return it
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var serializedToken = tokenHandler.WriteToken(token);

            return new Tokens
            {
                Token = serializedToken
            };
        }

        public void ValidateEmail(ValidateEmailRequest request)
        {
            var target = _dbContext.Users.FirstOrDefault(x =>
                x.Username == request.Username && x.SecurityToken == request.B64SecToken);

            if (target == null)
                throw new InvalidOperationException("Authentication failed");

            target.IsConfirmed = true;
            _dbContext.SaveChanges();
        }

        private bool Authenticate(string username, string password)
        {
            var target = _dbContext.Users.Single(x => x.Username == username);

            if (!target.IsConfirmed)
                return false;

            // Get stored salt and hash
            byte[] salt = Convert.FromBase64String(target.PwdSalt);
            byte[] hash = Convert.FromBase64String(target.PwdHash);

            byte[] calcHash =
                KeyDerivation.Pbkdf2(
                    password: password,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100000,
                    numBytesRequested: 256 / 8);

            return hash.SequenceEqual(calcHash);
        }
    }
}
