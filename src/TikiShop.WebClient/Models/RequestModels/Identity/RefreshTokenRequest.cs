using System.ComponentModel.DataAnnotations;

namespace TikiShop.WebClient.Models.RequestModels.Identity
{
    public sealed class RefreshTokenRequest
    {
        [Required]
        public string RefreshToken { get; set; }
    }
}