using Microsoft.AspNetCore.Identity;
using Org.BouncyCastle.Ocsp;
using TikiShop.Shared.RequestModels.Identity;
using TikiShop.Shared.ResponseModels.Identity;

namespace TikiShop.Api.Controllers
{
    [ApiController]
    [Route("/api/v1")]
    public class IdentityController : ControllerBase
    {
        private readonly IIdentityService _identityService;
        private readonly JwtConfig _jwtConfig;

        public IdentityController(
            IOptions<JwtConfig> jwtOptions,
            IIdentityService identityService)
        {
            _identityService = identityService;
            _jwtConfig = jwtOptions.Value;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("confirmEmail")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] ConfirmEmailRequest req)
        {
            var result = await _identityService.ConfirmEmail(req.Email, req.Code);
            return result.Succeeded ? Created() : Problem(result.Errors.FirstOrDefault());
        }


        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var email = this.User.FindFirstValue(ClaimTypes.Email);
            var result = await _identityService.Logout(email ?? "");
            if (!result.Succeeded)
            {
                return Problem(result.Errors.FirstOrDefault());
            }
            this.HttpContext.Response.Cookies.Delete("access-token");
            this.HttpContext.Response.Cookies.Delete("refresh-token");
            return Ok();
        }


        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest req)
        {
            var result = await _identityService.Register(req.UserName, req.Email, req.Password);
            return result.Succeeded ? Ok() : Problem(result.Errors.FirstOrDefault());
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest req)
        {
            var result = await _identityService.Login(req.Email, req.Password);
            if (!result.Succeeded)
            {
                return Problem(result.Errors.FirstOrDefault());
            }
            var tokensDto = result.Data!;
            SetTokenInCookie(tokensDto.AccessToken, tokensDto.RefreshToken);

            return Ok();
        }

        [ProducesResponseType(StatusCodes.Status302Found)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpGet]
        [Route("external-login")]
        public async Task<IActionResult> ExternalLogin([FromQuery] string provider, [FromQuery] string returnUrl)
        {
            var domainName = HttpContext.Request.Host.Value;
            var redirectUrl = $"https://{domainName}/api/v1/external-login-callback?returnUrl={returnUrl}";
            var result = await _identityService.ExternalLogin(provider, redirectUrl);
            return result.Succeeded ? Challenge(result.Data!, provider) : Problem(result.Errors.FirstOrDefault());
        }

        [ProducesResponseType(StatusCodes.Status302Found)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet]
        [Route("external-login-callback")]
        public async Task<IActionResult> ExternalLoginCallback([FromQuery] string returnUrl)
        {
            var result = await _identityService.ExternalLoginCallback();
            if (!result.Succeeded)
            {
                return Problem(result.Errors.FirstOrDefault());
            }
            var tokensDto = result.Data!;
            SetTokenInCookie(tokensDto.AccessToken, tokensDto.RefreshToken);
            return Redirect(returnUrl);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("refreshToken")]
        public async Task<IActionResult> RefreshToken()
        {
            this.HttpContext.Request.Cookies.TryGetValue("refresh-token", out var refreshToken);
            var result = await _identityService.RefreshToken(refreshToken ?? "");
            if (!result.Succeeded)
            {
                return Problem(result.Errors.FirstOrDefault());
            }
            var tokensDto = result.Data!;
            SetTokenInCookie(tokensDto.AccessToken, tokensDto.RefreshToken);
            return Ok();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("resendConfirmEmail")]
        public async Task<IActionResult> ResendConfirmEmail(ResendConfirmEmailRequest req)
        {
            var result = await _identityService.ResendConfirmEmail(req.Email);
            return result.Succeeded ? Ok() : Problem(result.Errors.FirstOrDefault());
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("forgotPassword")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest req)
        {
            var result = await _identityService.ForgotPassword(req.Email);
            return result.Succeeded ? Ok() : Problem(result.Errors.FirstOrDefault());
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("resetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest req)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.Sid);
            var result = await _identityService.ResetPassword(userId ?? "", req.ResetCode, req.NewPassword);
            if (!result.Succeeded)
            {
                return Problem(result.Errors.FirstOrDefault());
            }

            // Logout
            var email = this.User.FindFirstValue(ClaimTypes.Email);
            result = await _identityService.Logout(email ?? "");
            if (!result.Succeeded)
            {
                return Problem(result.Errors.FirstOrDefault());
            }
            this.HttpContext.Response.Cookies.Delete("access-token");
            this.HttpContext.Response.Cookies.Delete("refresh-token");
            return Ok();
        }


        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("changePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest req)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.Sid);
            var result = await _identityService.ChangePassword(userId ?? "", req.OldPassword, req.NewPassword);
            if (!result.Succeeded)
            {
                return Problem(result.Errors.FirstOrDefault());
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
