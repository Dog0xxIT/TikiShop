using TikiShop.Core.Services.AuthService.Commands;
using TikiShop.Model.Configurations;
using TikiShop.Model.Enums;
using TikiShop.Model.RequestModels.Identity;

using TikiShop.Model.ResponseModels.Identity;

/// <summary>
/// Controller responsible for authentication and authorization operations.
/// </summary>
[ApiController]
[Route("/api/v1")]
public class AuthController : ControllerBase
{
    private readonly JwtConfig _jwtConfig;
    private readonly IMediator _mediator;

    /// <summary>
    /// Constructor for AuthController.
    /// </summary>
    /// <param name="jwtOptions">JWT configuration options.</param>
    /// <param name="mediator">Mediator for handling commands.</param>
    public AuthController(
        IOptions<JwtConfig> jwtOptions,
        IMediator mediator)
    {
        _mediator = mediator;
        _jwtConfig = jwtOptions.Value;
    }

    /// <summary>
    /// Confirms a user's email with the provided code.
    /// </summary>
    /// <param name="req">The email confirmation request details.</param>
    /// <returns>An HTTP response indicating success or failure.</returns>
    [HttpGet("confirmEmail")]
    public async Task<IActionResult> ConfirmEmail([FromQuery] ConfirmEmailReq req)
    {
        var command = new ConfirmEmailCommand(req.Email, req.Code);
        var result = await _mediator.Send(command);
        if (result.ResultCode == ResultCode.Success) return Ok(result);
        return BadRequest(result);
    }

    /// <summary>
    /// Retrieves user management information including roles and email.
    /// </summary>
    /// <returns>Management information for the authenticated user.</returns>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet("manageInfo")]
    public IActionResult ManageInfo()
    {
        var userId = User.FindFirstValue(ClaimTypes.Sid);
        var email = User.FindFirstValue(ClaimTypes.Email);
        var roles = User.FindAll(ClaimTypes.Role);

        return Ok(new ManageInfoResp
        {
            UserId = userId,
            Email = email,
            Roles = roles.Select(role => role.Value).ToList()
        });
    }

    /// <summary>
    /// Logs the current user out and clears authentication cookies.
    /// </summary>
    /// <returns>An HTTP response indicating success or failure.</returns>
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

    /// <summary>
    /// Registers a new user account.
    /// </summary>
    /// <param name="req">The registration request details.</param>
    /// <returns>An HTTP response indicating success or failure.</returns>
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterReq req)
    {
        var command = new RegisterCommand(req.UserName, req.Email, req.Password);
        var result = await _mediator.Send(command);
        if (result.ResultCode == ResultCode.Success) return Ok(result);
        return BadRequest(result);
    }

    /// <summary>
    /// Logs a user in and sets authentication cookies.
    /// </summary>
    /// <param name="req">The login request details.</param>
    /// <returns>An HTTP response indicating success or failure.</returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginReq req)
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

    /// <summary>
    /// Initiates an external login flow.
    /// </summary>
    /// <param name="provider">The external provider (e.g., Google).</param>
    /// <param name="returnUrl">The URL to redirect to after login.</param>
    /// <returns>A redirection to the external login provider or an error response.</returns>
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

    /// <summary>
    /// Handles the callback for an external login flow.
    /// </summary>
    /// <param name="returnUrl">The URL to redirect to after login.</param>
    /// <returns>A redirection to the specified return URL or an error response.</returns>
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

    /// <summary>
    /// Refreshes authentication tokens using the refresh token.
    /// </summary>
    /// <returns>An HTTP response with new tokens or an error response.</returns>
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

    /// <summary>
    /// Resends an email confirmation request.
    /// </summary>
    /// <param name="req">The email resend request details.</param>
    /// <returns>An HTTP response indicating success or failure.</returns>
    [HttpPost("resendConfirmEmail")]
    public async Task<IActionResult> ResendConfirmEmail(ResendConfirmEmailReq req)
    {
        var command = new ResendConfirmEmailCommand(req.Email);
        var result = await _mediator.Send(command);
        if (result.ResultCode == ResultCode.Success) return Ok(result);
        return BadRequest(result);
    }

    /// <summary>
    /// Initiates the password recovery process.
    /// </summary>
    /// <param name="req">The password recovery request details.</param>
    /// <returns>An HTTP response indicating success or failure.</returns>
    [HttpPost("forgotPassword")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordReq req)
    {
        var command = new ForgotPasswordCommand(req.Email);
        var result = await _mediator.Send(command);
        if (result.ResultCode == ResultCode.Success) return Ok(result);
        return BadRequest(result);
    }

    /// <summary>
    /// Resets a user's password using a reset code.
    /// </summary>
    /// <param name="req">The password reset request details.</param>
    /// <returns>An HTTP response indicating success or failure.</returns>
    [HttpPost("resetPassword")]
    public async Task<IActionResult> ResetPassword(ResetPasswordReq req)
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

    /// <summary>
    /// Changes a user's password after verifying the old password.
    /// </summary>
    /// <param name="req">The password change request details.</param>
    /// <returns>An HTTP response indicating success or failure.</returns>
    [HttpPost("changePassword")]
    public async Task<IActionResult> ChangePassword(ChangePasswordReq req)
    {
        var changePasswordCommand = new ChangePasswordCommand(req.Email, req.OldPassword, req.NewPassword);
        var result = await _mediator.Send(changePasswordCommand);
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