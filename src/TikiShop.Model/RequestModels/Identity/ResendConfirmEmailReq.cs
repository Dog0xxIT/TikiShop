using System.ComponentModel.DataAnnotations;

namespace TikiShop.Model.RequestModels.Identity;

public sealed class ResendConfirmEmailReq
{
    [Required] [EmailAddress] public string Email { get; set; }
}