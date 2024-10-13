using Microsoft.AspNetCore.Components;
using TikiShop.Shared.RequestModels.Identity;
using TikiShop.WebClient.Core;

namespace TikiShop.WebClient.Pages.User;

public partial class Login
{
    [CascadingParameter(Name = "Notification")]
    private TelerikNotification _notificatioRef { get; set; }

    private LoginRequest _loginRequest;

    protected override void OnInitialized()
    {
        _loginRequest = new();
        // Check uri for redirect page
        if (_nav.Uri != $"{_nav.BaseUri}login")
        {
            _nav.NavigateTo("/login");
        }
    }

    private async Task ValidSubmit()
    {
        var resultObject = await _accountManagement.Login(_loginRequest);
        if (resultObject.ResultCode.Equals(ResultCode.Success))
        {
            _notificatioRef.Show(
                text: resultObject.Messages,
                themeColor: ThemeConstants.Notification.ThemeColor.Light);
            _nav.NavigateTo("/");
            return;
        }

        _notificatioRef.Show(
            text: resultObject.Messages,
            themeColor: ThemeConstants.Notification.ThemeColor.Error);
    }
}