using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TikiShop.Core.Configurations;

namespace TikiShop.Core.Services.TokenService;

public class TokenService : ITokenService
{
    private readonly JwtConfig _jwtConfig;
    private readonly ILogger<TokenService> _logger;

    public TokenService(ILogger<TokenService> logger, IOptions<JwtConfig> jwtConfigOptions)
    {
        _logger = logger;
        _jwtConfig = jwtConfigOptions.Value;
    }

    public string GenerateAccessToken(IEnumerable<Claim> claims)
    {
        if (claims == null || !claims.Any())
        {
            _logger.LogWarning("No claims provided for token generation.");
            throw new ArgumentException("Claims cannot be null or empty.", nameof(claims));
        }

        var signKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.SecretKey));
        var signingCredentials = new SigningCredentials(signKey, _jwtConfig.Algorithm);

        var optionsHeader = new Dictionary<string, string>
        {
            ["alg"] = signingCredentials.Algorithm,
            ["typ"] = JwtConstants.TokenType
        };
        var header = new JwtHeader(signingCredentials, optionsHeader);

        var payload = new JwtPayload(
            issuer: _jwtConfig.Issuer,
            audience: _jwtConfig.Audience,
            expires: DateTime.UtcNow.AddDays(Convert.ToInt64(_jwtConfig.Expires)),
            notBefore: DateTime.UtcNow,
            claims: claims);

        var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        var token = jwtSecurityTokenHandler.WriteToken(new JwtSecurityToken(header, payload));
        return token;
    }

    public string GenerateRefreshToken()
    {
        var refreshToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        _logger.LogInformation("Generated a new refresh token.");
        return refreshToken;
    }
}
