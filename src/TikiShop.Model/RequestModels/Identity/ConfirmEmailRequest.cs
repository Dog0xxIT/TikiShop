using System.ComponentModel.DataAnnotations;

namespace TikiShop.Model.RequestModels.Identity;

public sealed class ConfirmEmailRequest
{
    [Required] public string Code { get; set; }

    [Required] [EmailAddress] public string Email { get; set; }
}