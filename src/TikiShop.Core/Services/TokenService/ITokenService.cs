using System.Security.Claims;

namespace TikiShop.Core.Services.TokenService;

public interface ITokenService
{
    string GenerateAccessToken(IEnumerable<Claim> claims);
    string GenerateRefreshToken();
}