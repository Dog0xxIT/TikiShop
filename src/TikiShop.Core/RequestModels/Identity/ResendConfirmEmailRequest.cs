using System.ComponentModel.DataAnnotations;

namespace TikiShop.Core.RequestModels.Identity
{
    public sealed class ResendConfirmEmailRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}