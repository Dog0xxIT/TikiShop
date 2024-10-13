using Microsoft.AspNetCore.Components;
using TikiShop.Shared.RequestModels.Identity;
using TikiShop.WebClient.Core;

namespace TikiShop.WebClient.Pages.User;

public partial class ResetPassword
{
    [CascadingParameter(Name = "Notification")]
    private TelerikNotification _notificatioRef { get; set; }

    private ResetPasswordRequest _resetPasswordRequest;

    protected override void OnInitialized()
    {
        _resetPasswordRequest = new();
    }

    private async Task ValidSubmit()
    {
        var resultObject = await _identityService.ResetPassword(_resetPasswordRequest);
        if (resultObject.ResultCode.Equals(ResultCode.Success))
        {
            _notificatioRef.Show(
                text: resultObject.Messages,
                themeColor: ThemeConstants.Notification.ThemeColor.Light);
            _navManager.NavigateTo("/");
            return;
        }

        _notificatioRef.Show(
            text: resultObject.Messages,
            themeColor: ThemeConstants.Notification.ThemeColor.Error);
    }
}