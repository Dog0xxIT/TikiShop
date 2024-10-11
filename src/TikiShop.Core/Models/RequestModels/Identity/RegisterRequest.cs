using System.ComponentModel.DataAnnotations;

namespace TikiShop.Core.Models.RequestModels.Identity
{
    public sealed class RegisterRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string UserName { get; set; }
    }
}