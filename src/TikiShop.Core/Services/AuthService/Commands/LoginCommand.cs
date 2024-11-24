using System.Security.Claims;
using Microsoft.Extensions.Options;
using TikiShop.Core.Services.TokenService;
using TikiShop.Model.Configurations;
using TikiShop.Model.DTO;

namespace TikiShop.Core.Services.AuthService.Commands;

public record LoginCommand(string Email, string Password) : IRequest<ResultObject<TokensDto>>;

internal class LoginCommandHandler : IRequestHandler<LoginCommand, ResultObject<TokensDto>>
{
    private readonly JwtConfig _jwtConfig;
    private readonly ILogService<LoginCommandHandler> _logService;
    private readonly ITokenService _tokenService;
    private readonly UserManager<User> _userManager;

    public LoginCommandHandler(
        ILogService<LoginCommandHandler> logService,
        UserManager<User> userManager,
        ITokenService tokenService,
        IOptions<JwtConfig> jwtOptions)
    {
        _logService = logService;
        _userManager = userManager;
        _tokenService = tokenService;
        _jwtConfig = jwtOptions.Value;
    }

    public async Task<ResultObject<TokensDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        _logService.LogInformation($"Logging in user with email {request.Email}");
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null)
        {
            _logService.LogWarning($"Invalid login attempt for {request.Email}");
            return ResultObject<TokensDto>.Failed("Invalid Email");
        }

        var isConfirmEmail = await _userManager.IsEmailConfirmedAsync(user);
        if (!isConfirmEmail)
        {
            _logService.LogWarning($"Email not confirmed for {request.Email}");
            return ResultObject<TokensDto>.Failed("Please Confirm Email");
        }

        var isMatch = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!isMatch)
        {
            _logService.LogWarning($"Incorrect password for {request.Email}");
            return ResultObject<TokensDto>.Failed("Password incorrect");
        }

        var roles = await _userManager.GetRolesAsync(user);
        var claims = new List<Claim>
        {
            new(ClaimTypes.Sid, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email!)
        };
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var accessToken = _tokenService.GenerateAccessToken(claims);
        var refreshToken = _tokenService.GenerateRefreshToken();
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtConfig.RefreshTokenExpiryTime);
        var identityResult = await _userManager.UpdateAsync(user);
        if (!identityResult.Succeeded)
        {
            var errors = identityResult.Errors.Select(e => e.Description);
            _logService.LogError($"Failed to update user on login for {request.Email}: {string.Join(", ", errors)}");
            return ResultObject<TokensDto>.Failed(errors);
        }

        _logService.LogInformation($"User logged in successfully: {request.Email}");
        return ResultObject<TokensDto>.Success(new TokensDto(accessToken, refreshToken));
    }
}