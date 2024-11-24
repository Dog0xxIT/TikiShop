using System.Security.Claims;
using Microsoft.Extensions.Options;
using TikiShop.Core.Services.BasketService.Commands;
using TikiShop.Core.Services.TokenService;
using TikiShop.Core.Utils;
using TikiShop.Infrastructure.Common;
using TikiShop.Model.Configurations;
using TikiShop.Model.DTO;
using TikiShop.Model.Enums;

namespace TikiShop.Core.Services.AuthService.Commands;

public class ExternalLoginCallbackCommand : IRequest<ResultObject<TokensDto>>;

internal class
    ExternalLoginCallbackCommandHandler : IRequestHandler<ExternalLoginCallbackCommand, ResultObject<TokensDto>>
{
    private readonly JwtConfig _jwtConfig;
    private readonly ILogService<ExternalLoginCallbackCommandHandler> _logService;
    private readonly IMediator _mediator;
    private readonly SignInManager<User> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly UserManager<User> _userManager;

    public ExternalLoginCallbackCommandHandler(
        ILogService<ExternalLoginCallbackCommandHandler> logService,
        SignInManager<User> signInManager,
        UserManager<User> userManager,
        IMediator mediator,
        ITokenService tokenService,
        IOptions<JwtConfig> jwtOptions)
    {
        _logService = logService;
        _signInManager = signInManager;
        _userManager = userManager;
        _mediator = mediator;
        _tokenService = tokenService;
        _jwtConfig = jwtOptions.Value;
    }

    public async Task<ResultObject<TokensDto>> Handle(ExternalLoginCallbackCommand request,
        CancellationToken cancellationToken)
    {
        _logService.LogInformation("Processing external login callback.");
        var externalLoginInfo = await _signInManager.GetExternalLoginInfoAsync();
        if (externalLoginInfo is null)
        {
            _logService.LogWarning("External login info is null.");
            return ResultObject<TokensDto>.Failed("External Login Errors");
        }

        var user = await _userManager.FindByLoginAsync(externalLoginInfo.LoginProvider,
            externalLoginInfo.ProviderKey);
        if (user is null)
        {
            var userName = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Name) ?? "";
            user = new User
            {
                Email = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Email),
                UserName = Helper.RemoveUnicode(userName).Replace(" ", ""),
                EmailConfirmed = true,
                LockoutEnabled = false
            };

            var createResult = await _userManager.CreateAsync(user);
            if (!createResult.Succeeded)
            {
                var errors = createResult.Errors.Select(i => i.Description);
                _logService.LogError($"Failed to create user for external login: {string.Join(", ", errors)}");
                return ResultObject<TokensDto>.Failed(errors);
            }

            var resultAddRole = await _userManager.AddToRoleAsync(user, RolesConstant.Customer);
            if (!resultAddRole.Succeeded)
            {
                var errors = resultAddRole.Errors.Select(e => e.Description);
                _logService.LogError($"Failed to add role for user: {string.Join(", ", errors)}");
                return ResultObject<TokensDto>.Failed(errors);
            }

            // Create Basket For User
            var userId = await _userManager.GetUserIdAsync(user);
            var resultAddBasket = await _mediator.Send(new CreateBasketCommand(Convert.ToInt32(userId)));
            if (resultAddBasket.ResultCode == ResultCode.Failure)
            {
                _logService.LogError(
                    $"Failed to create basket for user: {string.Join(", ", resultAddBasket.Message)}");
                return ResultObject<TokensDto>.Failed(resultAddBasket.Message);
            }

            user = await _userManager.FindByEmailAsync(user.Email!);
            var userLoginInfo = new UserLoginInfo(
                externalLoginInfo.LoginProvider,
                externalLoginInfo.ProviderKey,
                externalLoginInfo.ProviderDisplayName);

            var loginResult = await _userManager.AddLoginAsync(user!, userLoginInfo);
            if (!loginResult.Succeeded)
            {
                var errors = loginResult.Errors.Select(i => i.Description);
                _logService.LogError($"Failed to add external login for user: {string.Join(", ", errors)}");
                return ResultObject<TokensDto>.Failed(errors);
            }
        }

        _signInManager.AuthenticationScheme = IdentityConstants.ExternalScheme;
        var result = await _signInManager.ExternalLoginSignInAsync(
            externalLoginInfo.LoginProvider,
            externalLoginInfo.ProviderKey,
            false);
        if (result.IsLockedOut)
        {
            _logService.LogWarning("User is locked out during external login.");
            return ResultObject<TokensDto>.Failed("Is Locked Out");
        }

        if (result.IsNotAllowed)
        {
            _logService.LogWarning("User is not allowed to log in during external login.");
            return ResultObject<TokensDto>.Failed("Is Not Allowed");
        }

        if (!result.Succeeded)
        {
            _logService.LogError("Server error during external login.");
            return ResultObject<TokensDto>.Failed("Server Error");
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
        user.RefreshTokenExpiryTime =
            DateTime.UtcNow.AddDays(_jwtConfig.RefreshTokenExpiryTime); // Expiry time refresh token
        var identityResult = await _userManager.UpdateAsync(user); // Save refresh token in Db
        if (!identityResult.Succeeded)
        {
            var errors = identityResult.Errors.Select(e => e.Description);
            _logService.LogError(
                $"Failed to update user on external login for user ID {user.Id}: {string.Join(", ", errors)}");
            return ResultObject<TokensDto>.Failed(errors);
        }

        _logService.LogInformation($"External login successful for user ID: {user.Id}");
        return ResultObject<TokensDto>.Success(new TokensDto(accessToken, refreshToken));
    }
}