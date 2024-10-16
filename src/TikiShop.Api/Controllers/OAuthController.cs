using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Routing;
using TikiShop.Core.Services;
using TikiShop.Core.Utils;

namespace TikiShop.Api.Controllers;

[ApiController]
[Route("")]
public class OAuthController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly IEmailSender<User> _emailSender;
    private readonly ILogger<OAuthController> _logger;
    private readonly JwtConfig _jwtConfig;
    private readonly SignInManager<User> _signInManager;

    public OAuthController(
        UserManager<User> userManager,
        IEmailSender<User> emailSender,
        ILogger<OAuthController> logger,
        IOptions<JwtConfig> jwtOptions,
        SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _emailSender = emailSender;
        _logger = logger;
        _signInManager = signInManager;
        _jwtConfig = jwtOptions.Value;
    }

    [HttpGet]
    [Route("external-login")]
    public async Task<IActionResult> ExternalLogin([FromQuery] string provider, [FromQuery] string returnUrl)
    {
        var authSchemes = await _signInManager.GetExternalAuthenticationSchemesAsync();
        if (authSchemes.All(s => s.Name != provider))
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest, detail: "Provider Invalid");
        }

        var domainName = HttpContext.Request.Host.Value;
        var redirectUrl = $"https://{domainName}/google-login?returnUrl={returnUrl}";
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        return Challenge(properties, provider);
    }

    [HttpGet]
    [Route("google-login")]
    public async Task<IActionResult> ExternalLoginCallback(string returnUrl)
    {
        var externalLoginInfo = await _signInManager.GetExternalLoginInfoAsync();
        if (externalLoginInfo is null)
        {
            return Problem("External Login Errors");
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
                return Problem(errors.FirstOrDefault());
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
                return Problem(errors.FirstOrDefault());
            }
        }

        _signInManager.AuthenticationScheme = IdentityConstants.ExternalScheme;
        var result = await _signInManager.ExternalLoginSignInAsync(
            externalLoginInfo.LoginProvider,
            externalLoginInfo.ProviderKey,
            false);
        if (result.IsLockedOut)
        {
            return Problem("Is Locked Out");
        }
        if (result.IsNotAllowed)
        {
            return Problem("Is Not Allowed");
        }
        if (!result.Succeeded)
        {
            return Problem("Server Error");
        }

        return Redirect(returnUrl);
    }
}