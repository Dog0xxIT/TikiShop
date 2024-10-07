using System.ComponentModel.DataAnnotations;

namespace TikiShop.Share.RequestModels.Identity
{
    public sealed class ResendConfirmEmailRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}