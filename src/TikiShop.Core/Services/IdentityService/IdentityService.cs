using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using TikiShop.Core.Configurations;
using TikiShop.Core.Dto;
using TikiShop.Core.Services.BasketService.Commands;
using TikiShop.Core.Services.EmailService;
using TikiShop.Core.Services.TokenService;
using TikiShop.Core.Utils;
using TikiShop.Infrastructure.Common;
using TikiShop.Infrastructure.Models;

namespace TikiShop.Core.Services.IdentityService
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<IdentityService> _logger;
        private readonly IEmailSender<User> _emailSender;
        private readonly ITokenService _tokenService;
        private readonly JwtConfig _jwtConfig;
        private readonly IMediator _mediator;

        public IdentityService(
            UserManager<User> userManager,
            ILogger<IdentityService> logger,
            IEmailSender<User> emailSender,
            ITokenService tokenService,
            IOptions<JwtConfig> jwtOptions,
            IMediator mediator,
            SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _logger = logger;
            _emailSender = emailSender;
            _tokenService = tokenService;
            _mediator = mediator;
            _signInManager = signInManager;
            _jwtConfig = jwtOptions.Value;
        }

        public async Task<ServiceResult> ConfirmEmail(string email, string code)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
            {
                return ServiceResult.Failed("Invalid Email");
            }
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var identityResult = await _userManager.ConfirmEmailAsync(user, code);
            if (!identityResult.Succeeded)
            {
                var errors = identityResult.Errors.Select(i => i.Description);
                return ServiceResult.Failed(errors);
            }

            return ServiceResult.Success;
        }

        public async Task<ServiceResult> Logout(string email)
        {
            var user = await _userManager.FindByEmailAsync(email!);
            if (user is null)
            {
                return ServiceResult.Failed("Request Invalid");
            }

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            var identityResult = await _userManager.UpdateAsync(user);
            if (!identityResult.Succeeded)
            {
                var errors = identityResult.Errors.Select(i => i.Description);
                return ServiceResult.Failed(errors);
            }

            return ServiceResult.Success;
        }

        public async Task<ServiceResult> Register(string userName, string email, string password)
        {
            var user = new User
            {
                Email = email,
                UserName = userName,
            };

            var identityResult = await _userManager.CreateAsync(user, password);
            if (!identityResult.Succeeded)
            {
                var errors = identityResult.Errors.Select(e => e.Description);
                return ServiceResult.Failed(errors);
            }

            identityResult = await _userManager.AddToRoleAsync(user, RolesConstant.Customer);
            if (!identityResult.Succeeded)
            {
                var errors = identityResult.Errors.Select(e => e.Description);
                return ServiceResult.Failed(errors);
            }

            // Create Basket For User
            var userId = await _userManager.GetUserIdAsync(user);
            var serviceResult = await _mediator.Send(new CreateBasketCommand(Convert.ToInt32(userId)));
            if (!serviceResult.Succeeded)
            {
                return ServiceResult.Failed(serviceResult.Errors);
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = EmailSenderGenerateLink.GenerateConfirmLink(token, email);
            await _emailSender.SendConfirmationLinkAsync(user, user.Email, confirmationLink);
            return ServiceResult.Success;
        }

        public async Task<ServiceResult<TokensDto>> Login(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
            {
                return ServiceResult<TokensDto>.Failed("Invalid Email");
            }

            var isConfirmEmail = await _userManager.IsEmailConfirmedAsync(user);
            if (!isConfirmEmail)
            {
                return ServiceResult<TokensDto>.Failed("Please Confirm Email");
            }

            // Use cookies await _signInManager.CheckPasswordSignInAsync(user, req.Password, req.RememberLogin); 
            // Use token    
            var isMatch = await _userManager.CheckPasswordAsync(user, password);
            if (!isMatch)
            {
                return ServiceResult<TokensDto>.Failed("Password incorrect");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
                new (ClaimTypes.Sid, user.Id.ToString()),
                new (ClaimTypes.Email, user.Email!),
            };
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var accessToken = _tokenService.GenerateAccessToken(claims);
            var refreshToken = _tokenService.GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtConfig.RefreshTokenExpiryTime); // Expiry time refresh token
            var identityResult = await _userManager.UpdateAsync(user); // Save refresh token in Db
            if (!identityResult.Succeeded)
            {
                var errors = identityResult.Errors.Select(e => e.Description);
                return ServiceResult<TokensDto>.Failed(errors);
            }

            return ServiceResult<TokensDto>.Success(new(accessToken, refreshToken));
        }

        public async Task<ServiceResult<TokensDto>> RefreshToken(string refreshToken)
        {
            var user = await _userManager.Users
                .SingleOrDefaultAsync(u => u.RefreshToken == refreshToken);

            if (user is null)
            {
                return ServiceResult<TokensDto>.Failed("Invalid token");
            }

            if (user.RefreshTokenExpiryTime is null || user.RefreshTokenExpiryTime < DateTime.UtcNow)
            {
                return ServiceResult<TokensDto>.Failed("Invalid token");
            }

            var claims = new List<Claim>
            {
                new (ClaimTypes.Sid, user.Id.ToString()),
                new (ClaimTypes.Email, user.Email!),
            };
            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var newAccessToken = _tokenService.GenerateAccessToken(claims);
            var newRefreshToken = _tokenService.GenerateRefreshToken();
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtConfig.RefreshTokenExpiryTime); // Expiry time refresh token
            var identityResult = await _userManager.UpdateAsync(user); // Save refresh token in Db
            if (!identityResult.Succeeded)
            {
                var errors = identityResult.Errors.Select(e => e.Description);
                return ServiceResult<TokensDto>.Failed(errors);
            }

            return ServiceResult<TokensDto>.Success(new(newAccessToken, newRefreshToken));
        }

        public async Task<ServiceResult> ResendConfirmEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
            {
                return ServiceResult.Failed("Invalid Email");
            }
            if (await _userManager.IsEmailConfirmedAsync(user))
            {
                return ServiceResult.Failed("Email confirmed");
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = EmailSenderGenerateLink.GenerateConfirmLink(token, email);
            await _emailSender.SendConfirmationLinkAsync(user, user.Email, confirmationLink);
            return ServiceResult.Success;
        }

        public async Task<ServiceResult> ForgotPassword(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
            {
                return ServiceResult.Failed("Invalid Email");
            }

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var passwordResetLink = EmailSenderGenerateLink.GenerateResetLink(code, email);
            await _emailSender.SendPasswordResetLinkAsync(user, email, passwordResetLink);

            return ServiceResult.Success;
        }

        public async Task<ServiceResult> ResetPassword(string userId, string resetCode, string newPassword)
        {
            var user = await _userManager.FindByEmailAsync(userId);
            if (user is null)
            {
                return ServiceResult.Failed("Invalid Email");
            }
            var identityResult = await _userManager.ResetPasswordAsync(user, resetCode, newPassword);

            if (!identityResult.Succeeded)
            {
                var errors = identityResult.Errors.Select(e => e.Description);
                return ServiceResult.Failed(errors);
            }

            return ServiceResult.Success;
        }

        public async Task<ServiceResult> ChangePassword(string userId, string oldPassword, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
            {
                return ServiceResult.Failed("Invalid User");
            }

            var identityResult = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
            if (!identityResult.Succeeded)
            {
                var errors = identityResult.Errors.Select(e => e.Description);
                return ServiceResult.Failed(errors);
            }

            return ServiceResult.Success;
        }

        public async Task<ServiceResult<AuthenticationProperties>> ExternalLogin(string provider, string redirectUrl)
        {
            var authSchemes = await _signInManager.GetExternalAuthenticationSchemesAsync();
            if (authSchemes.All(s => s.Name != provider))
            {
                return ServiceResult<AuthenticationProperties>.Failed("Provider Invalid");
            }
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return ServiceResult<AuthenticationProperties>.Success(properties);
        }

        public async Task<ServiceResult<TokensDto>> ExternalLoginCallback()
        {
            var externalLoginInfo = await _signInManager.GetExternalLoginInfoAsync();
            if (externalLoginInfo is null)
            {
                return ServiceResult<TokensDto>.Failed("External Login Errors");
            }

            var user = await _userManager.FindByLoginAsync(externalLoginInfo.LoginProvider, externalLoginInfo.ProviderKey);
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
                    return ServiceResult<TokensDto>.Failed(errors);
                }

                var resultAddRole = await _userManager.AddToRoleAsync(user, RolesConstant.Customer);
                if (!resultAddRole.Succeeded)
                {
                    var errors = resultAddRole.Errors.Select(e => e.Description);
                    return ServiceResult<TokensDto>.Failed(errors);
                }

                // Create Basket For User
                var userId = await _userManager.GetUserIdAsync(user);
                var resultAddBasket = await _mediator.Send(new CreateBasketCommand(Convert.ToInt32(userId)));
                if (!resultAddBasket.Succeeded)
                {
                    return ServiceResult<TokensDto>.Failed(resultAddBasket.Errors);
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
                    return ServiceResult<TokensDto>.Failed(errors);
                }
            }

            _signInManager.AuthenticationScheme = IdentityConstants.ExternalScheme;
            var result = await _signInManager.ExternalLoginSignInAsync(
                externalLoginInfo.LoginProvider,
                externalLoginInfo.ProviderKey,
                false);
            if (result.IsLockedOut)
            {
                return ServiceResult<TokensDto>.Failed("Is Locked Out");
            }
            if (result.IsNotAllowed)
            {
                return ServiceResult<TokensDto>.Failed("Is Not Allowed");
            }
            if (!result.Succeeded)
            {
                return ServiceResult<TokensDto>.Failed("Server Error");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
                new (ClaimTypes.Sid, user.Id.ToString()),
                new (ClaimTypes.Email, user.Email!),
            };
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var accessToken = _tokenService.GenerateAccessToken(claims);
            var refreshToken = _tokenService.GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtConfig.RefreshTokenExpiryTime); // Expiry time refresh token
            var identityResult = await _userManager.UpdateAsync(user); // Save refresh token in Db
            if (!identityResult.Succeeded)
            {
                var errors = identityResult.Errors.Select(e => e.Description);
                return ServiceResult<TokensDto>.Failed(errors);
            }

            return ServiceResult<TokensDto>.Success(new(accessToken, refreshToken));
        }
    }
}
