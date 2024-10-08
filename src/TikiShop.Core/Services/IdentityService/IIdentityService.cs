using System.Security.Claims;
using TikiShop.Core.Dto;
using TikiShop.Core.RequestModels.Identity;

namespace TikiShop.Core.Services.IdentityService;

public interface IIdentityService
{
    Task<ServiceResult> ConfirmEmail(string email, string code);
    Task<ServiceResult> Logout(string email);
    Task<ServiceResult> Register(string email, string password);
    Task<ServiceResult<TokensDto>> Login(string email, string password);
    Task<ServiceResult<TokensDto>> RefreshToken(string refreshToken);
    Task<ServiceResult> ResendConfirmEmail(string email);
    Task<ServiceResult> ForgotPassword(string email);
    Task<ServiceResult> ResetPassword(string userId, string resetCode, string newPassword);
    Task<ServiceResult> ChangePassword(string userId, string oldPassword, string newPassword);
}