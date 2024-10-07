using System.ComponentModel.DataAnnotations;

namespace TikiShop.WebClient.Models.RequestModels.Identity
{
    public sealed class ResendConfirmEmailRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}