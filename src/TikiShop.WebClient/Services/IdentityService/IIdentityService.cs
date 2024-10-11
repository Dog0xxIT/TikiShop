using TikiShop.WebClient.Core;
using TikiShop.WebClient.Models.RequestModels.Identity;
using TikiShop.WebClient.Models.ResponseModels.Common;

namespace TikiShop.WebClient.Services.IdentityService;

public interface IIdentityService
{
    Task<ResultObject<ResponseObject>> ConfirmEmail(ConfirmEmailRequest request);
    Task<ResultObject> Logout();
    Task<ResultObject<ResponseObject>> Register(RegisterRequest request);
    Task<ResultObject> Login(SignInRequest request);
    Task<ResultObject<ResponseObject>> ResendConfirmEmail(ResendConfirmEmailRequest request);
    Task<ResultObject<ResponseObject>> ForgotPassword(ForgotPasswordRequest request);
    Task<ResultObject<ResponseObject>> ResetPassword(ResetPasswordRequest request);
}