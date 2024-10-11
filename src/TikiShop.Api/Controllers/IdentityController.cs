using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TikiShop.Core.Configurations;
using TikiShop.Core.Models.RequestModels.Identity;
using TikiShop.Core.Models.ResponseModels.Identity;
using TikiShop.Core.Services.IdentityService;
using TikiShop.Core.Services.TokenService;
using TikiShop.Infrastructure.Models;

namespace TikiShop.Api.Controllers
{
    [ApiController]
    [Route("/api/v1")]
    public class IdentityController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IIdentityService _identityService;
        private readonly IEmailSender<User> _emailSender;
        private readonly ILogger<IdentityController> _logger;
        private readonly JwtConfig _jwtConfig;
        private readonly ITokenService _tokenService;

        public IdentityController(
            UserManager<User> userManager,
            IEmailSender<User> emailSender,
            ILogger<IdentityController> logger,
            IOptions<JwtConfig> jwtOptions,
            ITokenService tokenService,
            IIdentityService identityService)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _tokenService = tokenService;
            _identityService = identityService;
            _logger = logger;
            _jwtConfig = jwtOptions.Value;
        }

        [HttpGet("confirmEmail")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] ConfirmEmailRequest req)
        {
            var result = await _identityService.ConfirmEmail(req.Email, req.Code);
            if (!result.Succeeded)
            {
                return Problem(result.Errors.First());
            }

            return Ok();
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("manageInfo")]
        public IActionResult ManageInfo()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.Sid);
            var email = this.User.FindFirstValue(ClaimTypes.Email);
            var roles = this.User.FindAll(ClaimTypes.Role);

            return Ok(new ManageInfoResponse
            {
                UserId = userId,
                Email = email,
                Roles = roles.Select(role => role.Value).ToList(),
            });
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var email = this.User.FindFirstValue(ClaimTypes.Email);
            var result = await _identityService.Logout(email ?? "");
            if (!result.Succeeded)
            {
                return Problem(result.Errors.First());
            }
            this.HttpContext.Response.Cookies.Delete("access-token");
            this.HttpContext.Response.Cookies.Delete("refresh-token");

            return Ok();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest req)
        {
            var result = await _identityService.Register(req.UserName, req.Email, req.Password);
            if (!result.Succeeded)
            {
                return Problem(result.Errors.First());
            }

            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest req)
        {
            var result = await _identityService.Login(req.Email, req.Password);
            if (!result.Succeeded)
            {
                return Problem(result.Errors.First());
            }
            var tokensDto = result.Data!;
            SetTokenInCookie(tokensDto.AccessToken, tokensDto.RefreshToken);

            return Ok();
        }

        [HttpPost("refreshToken")]
        public async Task<IActionResult> RefreshToken()
        {
            this.HttpContext.Request.Cookies.TryGetValue("refresh-token", out var refreshToken);
            var result = await _identityService.RefreshToken(refreshToken ?? "");
            if (!result.Succeeded)
            {
                return Problem(result.Errors.First());
            }
            var tokensDto = result.Data!;
            SetTokenInCookie(tokensDto.AccessToken, tokensDto.RefreshToken);
            return Ok();
        }

        [HttpPost("resendConfirmEmail")]
        public async Task<IActionResult> ResendConfirmEmail(ResendConfirmEmailRequest req)
        {
            var result = await _identityService.ResendConfirmEmail(req.Email);
            if (!result.Succeeded)
            {
                return Problem(result.Errors.First());
            }

            return Ok();
        }

        [HttpPost("forgotPassword")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest req)
        {
            var result = await _identityService.ForgotPassword(req.Email);
            if (!result.Succeeded)
            {
                return Problem(result.Errors.First());
            }

            return Ok();
        }

        [HttpPost("resetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest req)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.Sid);
            var result = await _identityService.ResetPassword(userId ?? "", req.ResetCode, req.NewPassword);
            if (!result.Succeeded)
            {
                return Problem(result.Errors.First());
            }

            // Logout
            var email = this.User.FindFirstValue(ClaimTypes.Email);
            result = await _identityService.Logout(email ?? "");
            if (!result.Succeeded)
            {
                return Problem(result.Errors.First());
            }
            this.HttpContext.Response.Cookies.Delete("access-token");
            this.HttpContext.Response.Cookies.Delete("refresh-token");

            return Ok();
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("changePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest req)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.Sid);
            var result = await _identityService.ChangePassword(userId ?? "", req.OldPassword, req.NewPassword);
            if (!result.Succeeded)
            {
                return Problem(result.Errors.First());
            }

            // Logout
            var email = this.User.FindFirstValue(ClaimTypes.Email);
            result = await _identityService.Logout(email ?? "");
            if (!result.Succeeded)
            {
                return Problem(result.Errors.First());
            }
            this.HttpContext.Response.Cookies.Delete("access-token");
            this.HttpContext.Response.Cookies.Delete("refresh-token");

            return Ok();
        }

        private void SetTokenInCookie(string accessToken, string refreshToken)
        {
            var cookieOptionsForAccessToken = new CookieOptions()
            {
                HttpOnly = true, // XSS
                Secure = true, // Https
                Expires = DateTime.UtcNow.AddMinutes(_jwtConfig.Expires), // Expiration
                SameSite = SameSiteMode.Strict, // CSRF
            };
            this.HttpContext.Response.Cookies.Append("access-token", accessToken, cookieOptionsForAccessToken);

            var cookieOptionsForRefreshToken = new CookieOptions()
            {
                HttpOnly = true, // XSS
                Secure = true, // Https
                Expires = DateTime.UtcNow.AddDays(_jwtConfig.RefreshTokenExpiryTime), // Expiration
                SameSite = SameSiteMode.Strict, // CSRF
                Path = "/api/v1/refreshToken",
                Domain = "localhost"
            };
            this.HttpContext.Response.Cookies.Append("refresh-token", refreshToken, cookieOptionsForRefreshToken);
        }
    }
}
