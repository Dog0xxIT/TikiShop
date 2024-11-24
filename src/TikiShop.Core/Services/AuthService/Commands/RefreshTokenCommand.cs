using System.Security.Claims;
using Microsoft.Extensions.Options;
using TikiShop.Core.Services.TokenService;
using TikiShop.Model.Configurations;
using TikiShop.Model.DTO;

namespace TikiShop.Core.Services.AuthService.Commands;

public record RefreshTokenCommand(string RefreshToken) : IRequest<ResultObject<TokensDto>>;

internal class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, ResultObject<TokensDto>>
{
    private readonly JwtConfig _jwtConfig;
    private readonly ILogService<RefreshTokenCommandHandler> _logService;
    private readonly ITokenService _tokenService;
    private readonly UserManager<User> _userManager;

    public RefreshTokenCommandHandler(
        ILogService<RefreshTokenCommandHandler> logService,
        UserManager<User> userManager,
        ITokenService tokenService,
        IOptions<JwtConfig> jwtOptions)
    {
        _logService = logService;
        _userManager = userManager;
        _tokenService = tokenService;
        _jwtConfig = jwtOptions.Value;
    }

    public async Task<ResultObject<TokensDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        _logService.LogInformation($"Refreshing token for refresh token {request.RefreshToken}");
        var user = await _userManager.Users
            .SingleOrDefaultAsync(u => u.RefreshToken == request.RefreshToken);

        if (user is null)
        {
            _logService.LogWarning($"Invalid token attempt with token {request.RefreshToken}");
            return ResultObject<TokensDto>.Failed("Invalid token");
        }

        if (user.RefreshTokenExpiryTime is null || user.RefreshTokenExpiryTime < DateTime.UtcNow)
        {
            _logService.LogWarning($"Expired token used for refresh token {request.RefreshToken}");
            return ResultObject<TokensDto>.Failed("Invalid token");
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.Sid, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email!)
        };
        var roles = await _userManager.GetRolesAsync(user);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var newAccessToken = _tokenService.GenerateAccessToken(claims);
        var newRefreshToken = _tokenService.GenerateRefreshToken();
        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtConfig.RefreshTokenExpiryTime);
        var identityResult = await _userManager.UpdateAsync(user);
        if (!identityResult.Succeeded)
        {
            var errors = identityResult.Errors.Select(e => e.Description);
            _logService.LogError($"Failed to update user on token refresh: {string.Join(", ", errors)}");
            return ResultObject<TokensDto>.Failed(errors);
        }

        _logService.LogInformation($"Token refreshed successfully for user {user.Email}");
        return ResultObject<TokensDto>.Success(new TokensDto(newAccessToken, newRefreshToken));
    }
}