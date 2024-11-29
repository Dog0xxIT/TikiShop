using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TikiShop.Model.RequestModels.Identity;

public sealed class LoginReq
{
    [Required] [EmailAddress] public string Email { get; set; }

    [Required] [PasswordPropertyText] public string Password { get; set; }
}