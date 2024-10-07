using TikiShop.WebClient.Core;
using TikiShop.WebClient.Models.RequestModels.Identity;
using TikiShop.WebClient.Models.ResponseModels.Common;

namespace TikiShop.WebClient.States.AuthState
{
    public interface IAccountManagement
    {
        public Task<ResultObject<ResponseObject>> Login(SignInRequest req);

        public Task<ResultObject<ResponseObject>> Logout();
    }
}
