using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using TikiShop.Shared.RequestModels.Identity;
using TikiShop.WebClient.Core;
using TikiShop.WebClient.Services.IdentityService;

namespace TikiShop.WebClient.States.AuthState;

public class CookieAuthenticationStateProvider : AuthenticationStateProvider, IAccountManagement
{
    private readonly IIdentityService _identityService;

    /// <summary>
    /// Authentication state.
    /// </summary>
    private bool _authenticated;

    /// <summary>
    /// Default principal for anonymous (not authenticated) users.
    /// </summary>
    private readonly ClaimsPrincipal _unauthenticated = new(new ClaimsIdentity());

    public CookieAuthenticationStateProvider(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        _authenticated = false;
        // default to not authenticated
        var user = _unauthenticated;

        var result1 = await _identityService.ManageInfo();
        if (result1?.ResultCode != ResultCode.Success)
        {
            var result2 = await _identityService.RefreshToken();
            if (result2?.ResultCode != ResultCode.Success)
            {
                return new AuthenticationState(user);
            }
        }

        var resultData = result1!.Data;
        var claims = new List<Claim>
        {
            new(ClaimTypes.Sid, resultData.UserId),
            new(ClaimTypes.Email, resultData.Email)
        };

        claims.AddRange(resultData.Roles.Select(role =>
            new Claim(ClaimTypes.Role, role)));
        var id = new ClaimsIdentity(claims, nameof(CookieAuthenticationStateProvider));
        user = new ClaimsPrincipal(id);
        _authenticated = true;
        return new AuthenticationState(user);
    }

    public async Task<ResultObject> Login(LoginRequest req)
    {
        var resultObject = await _identityService.Login(req);
        if (resultObject.ResultCode.Equals(ResultCode.Success))
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
        return resultObject;
    }

    public async Task<ResultObject> Logout()
    {
        var resultObject = await _identityService.Logout();
        if (resultObject.ResultCode.Equals(ResultCode.Success))
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
        return resultObject;
    }
}