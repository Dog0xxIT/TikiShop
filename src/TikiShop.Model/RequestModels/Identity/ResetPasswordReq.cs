using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TikiShop.Model.RequestModels.Identity;

public sealed class ResetPasswordReq
{
    [Required] [EmailAddress] public string Email { get; set; }
    [Required] public string ResetCode { get; set; }

    [Required] [PasswordPropertyText] public string NewPassword { get; set; }
}