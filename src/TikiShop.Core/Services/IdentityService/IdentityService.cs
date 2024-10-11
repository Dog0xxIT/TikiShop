using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using TikiShop.Core.Configurations;
using TikiShop.Core.Dto;
using TikiShop.Core.Services.EmailService;
using TikiShop.Core.Services.TokenService;
using TikiShop.Infrastructure.Common;
using TikiShop.Infrastructure.Models;

namespace TikiShop.Core.Services.IdentityService
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<User> _userManager;
        private readonly ILogger<IdentityService> _logger;
        private readonly IEmailSender<User> _emailSender;
        private readonly ITokenService _tokenService;
        private readonly JwtConfig _jwtConfig;

        public IdentityService(
            UserManager<User> userManager,
            ILogger<IdentityService> logger,
            IEmailSender<User> emailSender,
            ITokenService tokenService,
            IOptions<JwtConfig> jwtOptions)
        {
            _userManager = userManager;
            _logger = logger;
            _emailSender = emailSender;
            _tokenService = tokenService;
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
            // Use cookies await _signInManager.CheckPasswordSignInAsync(user, req.Password, req.RememberLogin); 
            // Use token    
            var isMatch = await _userManager.CheckPasswordAsync(user, password);
            if (!isMatch)
            {
                return ServiceResult<TokensDto>.Failed("Password incorrect");
            }

            var claims = new List<Claim>
            {
                new (ClaimTypes.Sid, user.Id.ToString()),
                new (ClaimTypes.Email, user.Email!),
            };

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
    }
}
