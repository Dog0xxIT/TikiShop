using TikiShop.Shared.RequestModels.Identity;
using TikiShop.Shared.ResponseModels.Identity;
using TikiShop.WebClient.Core;

namespace TikiShop.WebClient.Services.IdentityService;

public interface IIdentityService
{
    Task<ResultObject> ConfirmEmail(ConfirmEmailRequest request);
    Task<ResultObject<ManageInfoResponse>> ManageInfo();
    Task<ResultObject> Logout();
    Task<ResultObject> Register(RegisterRequest request);
    Task<ResultObject> Login(LoginRequest request);
    Task<ResultObject> RefreshToken();
    Task<ResultObject> ResendConfirmEmail(ResendConfirmEmailRequest request);
    Task<ResultObject> ForgotPassword(ForgotPasswordRequest request);
    Task<ResultObject> ResetPassword(ResetPasswordRequest request);
    Task<ResultObject> ChangePassword(ResetPasswordRequest request);
}