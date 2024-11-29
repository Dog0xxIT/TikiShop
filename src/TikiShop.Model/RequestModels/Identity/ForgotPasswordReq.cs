using System.ComponentModel.DataAnnotations;

namespace TikiShop.Model.RequestModels.Identity;

public sealed class ForgotPasswordReq
{
    [Required] [EmailAddress] public string Email { get; set; }
}