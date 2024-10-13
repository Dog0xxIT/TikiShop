using TikiShop.Shared.RequestModels.Identity;
using TikiShop.WebClient.Core;

namespace TikiShop.WebClient.States.AuthState
{
    public interface IAccountManagement
    {
        public Task<ResultObject> Login(LoginRequest req);

        public Task<ResultObject> Logout();
    }
}
