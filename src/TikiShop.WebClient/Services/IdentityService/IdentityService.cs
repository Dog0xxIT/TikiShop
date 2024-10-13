using TikiShop.Shared.RequestModels.Identity;
using TikiShop.Shared.ResponseModels.Identity;
using TikiShop.WebClient.Core;
using TikiShop.WebClient.Core.CoreHttpClient;

namespace TikiShop.WebClient.Services.IdentityService
{
    public class IdentityService : IIdentityService
    {
        private readonly ICoreHttpClient _coreHttpClient;

        public IdentityService(ICoreHttpClient coreHttpClient)
        {
            _coreHttpClient = coreHttpClient;
        }

        public async Task<ResultObject> ConfirmEmail(ConfirmEmailRequest request)
        {
            return await _coreHttpClient.PostAsync(
                clientName: "TikiShopApi",
                uri: "/api/v1/confirmEmail",
                reqObj: request);
        }

        public async Task<ResultObject<ManageInfoResponse>> ManageInfo()
        {
            return await _coreHttpClient.GetAsync<ManageInfoResponse>(
                clientName: "TikiShopApi",
                uri: "/api/v1/manageInfo");
        }

        public async Task<ResultObject> Register(RegisterRequest request)
        {
            return await _coreHttpClient.PostAsync(
                clientName: "TikiShopApi",
                uri: "/api/v1/register",
                reqObj: request);
        }

        public async Task<ResultObject> Login(LoginRequest request)
        {
            return await _coreHttpClient.PostAsync(
                clientName: "TikiShopApi",
                uri: "/api/v1/login",
                reqObj: request);
        }

        public async Task<ResultObject> RefreshToken()
        {
            return await _coreHttpClient.PostAsync(
                clientName: "TikiShopApi",
                uri: "/api/v1/login",
                reqObj: null);
        }

        public async Task<ResultObject> Logout()
        {
            return await _coreHttpClient.PostAsync(
                clientName: "TikiShopApi",
                uri: "/api/v1/logout",
                reqObj: null);
        }

        public async Task<ResultObject> ResendConfirmEmail(ResendConfirmEmailRequest request)
        {
            return await _coreHttpClient.PostAsync(
                clientName: "TikiShopApi",
                uri: "/api/v1/resendConfirmEmail",
                reqObj: request);
        }

        public async Task<ResultObject> ForgotPassword(ForgotPasswordRequest request)
        {
            return await _coreHttpClient.PostAsync(
                clientName: "TikiShopApi",
                uri: "/api/v1/forgotPassword",
                reqObj: request);
        }

        public async Task<ResultObject> ResetPassword(ResetPasswordRequest request)
        {
            return await _coreHttpClient.PostAsync(
                clientName: "TikiShopApi",
                uri: "/api/v1/resetPassword",
                reqObj: request);
        }

        public Task<ResultObject> ChangePassword(ResetPasswordRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
