using Microsoft.AspNetCore.Components;
using TikiShop.WebClient.Core;
using TikiShop.WebClient.Models.RequestModels.Identity;

namespace TikiShop.WebClient.Pages.User;

public partial class ForgotPassword
{
    [CascadingParameter(Name = "Notification")]
    private TelerikNotification _notificatioRef { get; set; }

    private ForgotPasswordRequest _forgotPasswordRequest;

    protected override void OnInitialized()
    {
        _forgotPasswordRequest = new();
    }

    private async Task ValidSubmit()
    {
        var resultObject = await _identityService.ForgotPassword(_forgotPasswordRequest);
        if (resultObject.ResultCode.Equals(ResultCode.Success))
        {
            _notificatioRef.Show(
                text: "Please check email",
                themeColor: ThemeConstants.Notification.ThemeColor.Light);
            _navManager.NavigateTo("/");
            return;
        }

        _notificatioRef.Show(
            text: resultObject.Messages,
            themeColor: ThemeConstants.Notification.ThemeColor.Error);
    }
}