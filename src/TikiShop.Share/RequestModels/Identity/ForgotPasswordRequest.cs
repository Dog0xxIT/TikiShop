using System.ComponentModel.DataAnnotations;

namespace TikiShop.Share.RequestModels.Identity
{
    public sealed class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}