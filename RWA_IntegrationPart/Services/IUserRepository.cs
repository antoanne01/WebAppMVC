using RWA_IntegrationPart.Model;
using RWA_IntegrationPart.Models;

namespace RWA_IntegrationPart.Services
{
    public interface IUserRepository
    {
        User Add(UserRegisterRequest request);
        void ValidateEmail(ValidateEmailRequest request);
        Tokens JwtTokens(JwtTokensRequest request);
    }
}
