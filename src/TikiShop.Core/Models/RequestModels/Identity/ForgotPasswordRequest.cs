using System.ComponentModel.DataAnnotations;

namespace TikiShop.Core.Models.RequestModels.Identity
{
    public sealed class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}