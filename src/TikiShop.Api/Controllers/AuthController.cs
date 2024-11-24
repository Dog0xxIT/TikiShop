using TikiShop.Core.Services.AuthService.Commands;
using TikiShop.Model.Configurations;
using TikiShop.Model.Enums;
using TikiShop.Model.RequestModels.Identity;
using TikiShop.Model.ResponseModels.Identity;

namespace TikiShop.Api.Controllers;

[ApiController]
[Route("/api/v1")]
public class AuthController : ControllerBase
{
    private readonly JwtConfig _jwtConfig;
    private readonly IMediator _mediator;

    public AuthController(
        IOptions<JwtConfig> jwtOptions,
        IMediator mediator)
    {
        _mediator = mediator;
        _jwtConfig = jwtOptions.Value;
    }


    [HttpGet("confirmEmail")]
    public async Task<IActionResult> ConfirmEmail([FromQuery] ConfirmEmailRequest req)
    {
        var command = new ConfirmEmailCommand(req.Email, req.Code);
        var result = await _mediator.Send(command);
        if (result.ResultCode == ResultCode.Success) return Ok(result);
        return BadRequest(result);
    }


    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet("manageInfo")]
    public IActionResult ManageInfo()
    {
        var userId = User.FindFirstValue(ClaimTypes.Sid);
        var email = User.FindFirstValue(ClaimTypes.Email);
        var roles = User.FindAll(ClaimTypes.Role);

        return Ok(new ManageInfoResponse
        {
            UserId = userId,
            Email = email,
            Roles = roles.Select(role => role.Value).ToList()
        });
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var email = User.FindFirstValue(ClaimTypes.Email);
        var command = new LogoutCommand(email);
        var result = await _mediator.Send(command);
        if (result.ResultCode == ResultCode.Success)
        {
            HttpContext.Response.Cookies.Delete("access-token");
            HttpContext.Response.Cookies.Delete("refresh-token");
            return Ok(result);
        }

        return Unauthorized();
    }


    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest req)
    {
        var command = new RegisterCommand(req.UserName, req.Email, req.Password);
        var result = await _mediator.Send(command);
        if (result.ResultCode == ResultCode.Success) return Ok(result);
        return BadRequest(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest req)
    {
        var command = new LoginCommand(req.Email, req.Password);
        var result = await _mediator.Send(command);
        if (result.ResultCode == ResultCode.Success)
        {
            var tokensDto = result.Data!;
            SetTokenInCookie(tokensDto.AccessToken, tokensDto.RefreshToken);
            return Ok();
        }

        return BadRequest(result);
    }


    [HttpGet]
    [Route("external-login")]
    public async Task<IActionResult> ExternalLogin([FromQuery] string provider, [FromQuery] string returnUrl)
    {
        var domainName = HttpContext.Request.Host.Value;
        var redirectUrl = $"https://{domainName}/api/v1/external-login-callback?returnUrl={returnUrl}";
        var command = new ExternalLoginCommand(provider, redirectUrl);
        var result = await _mediator.Send(command);
        if (result.ResultCode == ResultCode.Success) return Challenge(result.Data!, provider);
        return BadRequest(result);
    }


    [HttpGet]
    [Route("external-login-callback")]
    public async Task<IActionResult> ExternalLoginCallback([FromQuery] string returnUrl)
    {
        var command = new ExternalLoginCallbackCommand();
        var result = await _mediator.Send(command);
        if (result.ResultCode == ResultCode.Success)
        {
            var tokensDto = result.Data!;
            SetTokenInCookie(tokensDto.AccessToken, tokensDto.RefreshToken);
            return Redirect(returnUrl);
        }

        return BadRequest(result);
    }


    [HttpPost("refreshToken")]
    public async Task<IActionResult> RefreshToken()
    {
        HttpContext.Request.Cookies.TryGetValue("refresh-token", out var refreshToken);
        var command = new RefreshTokenCommand(refreshToken ?? "");
        var result = await _mediator.Send(command);
        if (result.ResultCode == ResultCode.Success)
        {
            var tokensDto = result.Data!;
            SetTokenInCookie(tokensDto.AccessToken, tokensDto.RefreshToken);
            return Ok();
        }

        return BadRequest(result);
    }

    [HttpPost("resendConfirmEmail")]
    public async Task<IActionResult> ResendConfirmEmail(ResendConfirmEmailRequest req)
    {
        var command = new ResendConfirmEmailCommand(req.Email);
        var result = await _mediator.Send(command);
        if (result.ResultCode == ResultCode.Success) return Ok(result);
        return BadRequest(result);
    }


    [HttpPost("forgotPassword")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest req)
    {
        var command = new ForgotPasswordCommand(req.Email);
        var result = await _mediator.Send(command);
        if (result.ResultCode == ResultCode.Success) return Ok(result);
        return BadRequest(result);
    }

    [HttpPost("resetPassword")]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequest req)
    {
        var resetPasswordCommand = new ResetPasswordCommand(req.Email, req.ResetCode, req.NewPassword);
        var result = await _mediator.Send(resetPasswordCommand);
        if (result.ResultCode == ResultCode.Failure) return BadRequest(result);

        // Logout
        var logoutCommand = new LogoutCommand(req.Email);
        result = await _mediator.Send(logoutCommand);
        if (result.ResultCode == ResultCode.Success)
        {
            HttpContext.Response.Cookies.Delete("access-token");
            HttpContext.Response.Cookies.Delete("refresh-token");
            return Ok();
        }

        return BadRequest(result);
    }

    [HttpPost("changePassword")]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest req)
    {
        var changePasswordCommand = new ChangePasswordCommand(req.Email, req.OldPassword, req.NewPassword);
        var result = await _mediator.Send(changePasswordCommand);
        if (result.ResultCode == ResultCode.Failure) return BadRequest(result);

        // Logout
        var email = User.FindFirstValue(ClaimTypes.Email);
        var logoutCommand = new LogoutCommand(req.Email);
        result = await _mediator.Send(logoutCommand);
        if (result.ResultCode == ResultCode.Success)
        {
            HttpContext.Response.Cookies.Delete("access-token");
            HttpContext.Response.Cookies.Delete("refresh-token");
            return Ok();
        }

        return BadRequest(result);
    }

    private void SetTokenInCookie(string accessToken, string refreshToken)
    {
        var cookieOptionsForAccessToken = new CookieOptions
        {
            HttpOnly = true, // XSS
            Secure = true, // Https
            Expires = DateTime.UtcNow.AddMinutes(_jwtConfig.Expires), // Expiration
            SameSite = SameSiteMode.Strict // CSRF
        };
        HttpContext.Response.Cookies.Append("access-token", accessToken, cookieOptionsForAccessToken);

        var cookieOptionsForRefreshToken = new CookieOptions
        {
            HttpOnly = true, // XSS
            Secure = true, // Https
            Expires = DateTime.UtcNow.AddDays(_jwtConfig.RefreshTokenExpiryTime), // Expiration
            SameSite = SameSiteMode.Strict, // CSRF
            Path = "/api/v1/refreshToken",
            Domain = "localhost"
        };
        HttpContext.Response.Cookies.Append("refresh-token", refreshToken, cookieOptionsForRefreshToken);
    }
}