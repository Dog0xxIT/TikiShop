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
            _logger.LogInformation($"Confirming email for {email}");
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
            {
                _logger.LogWarning($"Invalid email confirmation attempt for {email}");
                return ServiceResult.Failed("Invalid Email");
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var identityResult = await _userManager.ConfirmEmailAsync(user, code);
            if (!identityResult.Succeeded)
            {
                var errors = identityResult.Errors.Select(i => i.Description);
                _logger.LogError($"Email confirmation failed for {email}: {string.Join(", ", errors)}");
                return ServiceResult.Failed(errors);
            }

            _logger.LogInformation($"Email confirmed for {email}");
            return ServiceResult.Success;
        }

        public async Task<ServiceResult> Logout(string email)
        {
            _logger.LogInformation($"Logging out user with email {email}");
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
            {
                _logger.LogWarning($"Logout attempt for invalid email {email}");
                return ServiceResult.Failed("Request Invalid");
            }

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            var identityResult = await _userManager.UpdateAsync(user);
            if (!identityResult.Succeeded)
            {
                var errors = identityResult.Errors.Select(i => i.Description);
                _logger.LogError($"Logout failed for {email}: {string.Join(", ", errors)}");
                return ServiceResult.Failed(errors);
            }

            _logger.LogInformation($"User logged out successfully: {email}");
            return ServiceResult.Success;
        }

        public async Task<ServiceResult> Register(string userName, string email, string password)
        {
            _logger.LogInformation($"Registering user with email {email}");
            var user = new User
            {
                Email = email,
                UserName = userName
            };

            var identityResult = await _userManager.CreateAsync(user, password);
            if (!identityResult.Succeeded)
            {
                var errors = identityResult.Errors.Select(e => e.Description);
                _logger.LogError($"User registration failed for {email}: {string.Join(", ", errors)}");
                return ServiceResult.Failed(errors);
            }

            identityResult = await _userManager.AddToRoleAsync(user, RolesConstant.Customer);
            if (!identityResult.Succeeded)
            {
                var errors = identityResult.Errors.Select(e => e.Description);
                _logger.LogError($"Failed to add role for {email}: {string.Join(", ", errors)}");
                return ServiceResult.Failed(errors);
            }

            var userCreated = await _userManager.Users
                .AsNoTracking()
                .Select(u => new { Email = u.Email, Id = u.Id })
                .SingleAsync(u => u.Email == user.Email);
            var serviceResult = await _mediator.Send(new CreateBasketCommand(Convert.ToInt32(userCreated.Id)));
            if (!serviceResult.Succeeded)
            {
                _logger.LogError($"Failed to create basket for {email}: {string.Join(", ", serviceResult.Errors)}");
                return ServiceResult.Failed(serviceResult.Errors);
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = EmailSenderGenerateLink.GenerateConfirmLink(token, email);
            await _emailSender.SendConfirmationLinkAsync(user, user.Email, confirmationLink);
            _logger.LogInformation($"Confirmation email sent to {email}");
            return ServiceResult.Success;
        }

        public async Task<ServiceResult<TokensDto>> Login(string email, string password)
        {
            _logger.LogInformation($"Logging in user with email {email}");
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
            {
                _logger.LogWarning($"Invalid login attempt for {email}");
                return ServiceResult<TokensDto>.Failed("Invalid Email");
            }

            var isConfirmEmail = await _userManager.IsEmailConfirmedAsync(user);
            if (!isConfirmEmail)
            {
                _logger.LogWarning($"Email not confirmed for {email}");
                return ServiceResult<TokensDto>.Failed("Please Confirm Email");
            }

            var isMatch = await _userManager.CheckPasswordAsync(user, password);
            if (!isMatch)
            {
                _logger.LogWarning($"Incorrect password for {email}");
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
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtConfig.RefreshTokenExpiryTime);
            var identityResult = await _userManager.UpdateAsync(user);
            if (!identityResult.Succeeded)
            {
                var errors = identityResult.Errors.Select(e => e.Description);
                _logger.LogError($"Failed to update user on login for {email}: {string.Join(", ", errors)}");
                return ServiceResult<TokensDto>.Failed(errors);
            }

            _logger.LogInformation($"User logged in successfully: {email}");
            return ServiceResult<TokensDto>.Success(new(accessToken, refreshToken));
        }

        public async Task<ServiceResult<TokensDto>> RefreshToken(string refreshToken)
        {
            _logger.LogInformation($"Refreshing token for refresh token {refreshToken}");
            var user = await _userManager.Users
                .SingleOrDefaultAsync(u => u.RefreshToken == refreshToken);

            if (user is null)
            {
                _logger.LogWarning($"Invalid token attempt with token {refreshToken}");
                return ServiceResult<TokensDto>.Failed("Invalid token");
            }

            if (user.RefreshTokenExpiryTime is null || user.RefreshTokenExpiryTime < DateTime.UtcNow)
            {
                _logger.LogWarning($"Expired token used for refresh token {refreshToken}");
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
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtConfig.RefreshTokenExpiryTime);
            var identityResult = await _userManager.UpdateAsync(user);
            if (!identityResult.Succeeded)
            {
                var errors = identityResult.Errors.Select(e => e.Description);
                _logger.LogError($"Failed to update user on token refresh: {string.Join(", ", errors)}");
                return ServiceResult<TokensDto>.Failed(errors);
            }

            _logger.LogInformation($"Token refreshed successfully for user {user.Email}");
            return ServiceResult<TokensDto>.Success(new(newAccessToken, newRefreshToken));
        }

        public async Task<ServiceResult> ResendConfirmEmail(string email)
        {
            _logger.LogInformation($"Resending confirmation email to {email}");
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
            {
                _logger.LogWarning($"Invalid email for confirmation resend: {email}");
                return ServiceResult.Failed("Invalid Email");
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = EmailSenderGenerateLink.GenerateConfirmLink(token, email);
            await _emailSender.SendConfirmationLinkAsync(user, user.Email, confirmationLink);
            _logger.LogInformation($"Confirmation email resent to {email}");
            return ServiceResult.Success;
        }

        public async Task<ServiceResult> ForgotPassword(string email)
        {
            _logger.LogInformation($"Initiating password reset for email: {email}");
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
            {
                _logger.LogWarning($"Password reset attempt for invalid email: {email}");
                return ServiceResult.Failed("Invalid Email");
            }

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var passwordResetLink = EmailSenderGenerateLink.GenerateResetLink(code, email);
            await _emailSender.SendPasswordResetLinkAsync(user, email, passwordResetLink);

            _logger.LogInformation($"Password reset link sent to email: {email}");
            return ServiceResult.Success;
        }

        public async Task<ServiceResult> ResetPassword(string userId, string resetCode, string newPassword)
        {
            _logger.LogInformation($"Resetting password for user ID: {userId}");
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
            {
                _logger.LogWarning($"Reset password attempt for invalid user ID: {userId}");
                return ServiceResult.Failed("Invalid Email");
            }

            var identityResult = await _userManager.ResetPasswordAsync(user, resetCode, newPassword);
            if (!identityResult.Succeeded)
            {
                var errors = identityResult.Errors.Select(e => e.Description);
                _logger.LogError($"Failed to reset password for user ID {userId}: {string.Join(", ", errors)}");
                return ServiceResult.Failed(errors);
            }

            _logger.LogInformation($"Password reset successfully for user ID: {userId}");
            return ServiceResult.Success;
        }

        public async Task<ServiceResult> ChangePassword(string userId, string oldPassword, string newPassword)
        {
            _logger.LogInformation($"Changing password for user ID: {userId}");
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
            {
                _logger.LogWarning($"Change password attempt for invalid user ID: {userId}");
                return ServiceResult.Failed("Invalid User");
            }

            var identityResult = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
            if (!identityResult.Succeeded)
            {
                var errors = identityResult.Errors.Select(e => e.Description);
                _logger.LogError($"Failed to change password for user ID {userId}: {string.Join(", ", errors)}");
                return ServiceResult.Failed(errors);
            }

            _logger.LogInformation($"Password changed successfully for user ID: {userId}");
            return ServiceResult.Success;
        }

        public async Task<ServiceResult<AuthenticationProperties>> ExternalLogin(string provider, string redirectUrl)
        {
            _logger.LogInformation($"Initiating external login with provider: {provider}");
            var authSchemes = await _signInManager.GetExternalAuthenticationSchemesAsync();
            if (authSchemes.All(s => s.Name != provider))
            {
                _logger.LogWarning($"Invalid external login provider: {provider}");
                return ServiceResult<AuthenticationProperties>.Failed("Provider Invalid");
            }

            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            _logger.LogInformation($"Configured external authentication properties for provider: {provider}");
            return ServiceResult<AuthenticationProperties>.Success(properties);
        }

        public async Task<ServiceResult<TokensDto>> ExternalLoginCallback()
        {
            _logger.LogInformation("Processing external login callback.");
            var externalLoginInfo = await _signInManager.GetExternalLoginInfoAsync();
            if (externalLoginInfo is null)
            {
                _logger.LogWarning("External login info is null.");
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
                    _logger.LogError($"Failed to create user for external login: {string.Join(", ", errors)}");
                    return ServiceResult<TokensDto>.Failed(errors);
                }

                var resultAddRole = await _userManager.AddToRoleAsync(user, RolesConstant.Customer);
                if (!resultAddRole.Succeeded)
                {
                    var errors = resultAddRole.Errors.Select(e => e.Description);
                    _logger.LogError($"Failed to add role for user: {string.Join(", ", errors)}");
                    return ServiceResult<TokensDto>.Failed(errors);
                }

                // Create Basket For User
                var userId = await _userManager.GetUserIdAsync(user);
                var resultAddBasket = await _mediator.Send(new CreateBasketCommand(Convert.ToInt32(userId)));
                if (!resultAddBasket.Succeeded)
                {
                    _logger.LogError($"Failed to create basket for user: {string.Join(", ", resultAddBasket.Errors)}");
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
                    _logger.LogError($"Failed to add external login for user: {string.Join(", ", errors)}");
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
                _logger.LogWarning("User is locked out during external login.");
                return ServiceResult<TokensDto>.Failed("Is Locked Out");
            }
            if (result.IsNotAllowed)
            {
                _logger.LogWarning("User is not allowed to log in during external login.");
                return ServiceResult<TokensDto>.Failed("Is Not Allowed");
            }
            if (!result.Succeeded)
            {
                _logger.LogError("Server error during external login.");
                return ServiceResult<TokensDto>.Failed("Server Error");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
                new(ClaimTypes.Sid, user.Id.ToString()),
                new(ClaimTypes.Email, user.Email!),
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
                _logger.LogError($"Failed to update user on external login for user ID {user.Id}: {string.Join(", ", errors)}");
                return ServiceResult<TokensDto>.Failed(errors);
            }

            _logger.LogInformation($"External login successful for user ID: {user.Id}");
            return ServiceResult<TokensDto>.Success(new(accessToken, refreshToken));
        }
    }
}
