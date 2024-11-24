using System.ComponentModel.DataAnnotations;

namespace TikiShop.Model.RequestModels.Identity;

public sealed class ResendConfirmEmailRequest
{
    [Required] [EmailAddress] public string Email { get; set; }
}