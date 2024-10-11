using System.ComponentModel.DataAnnotations;

namespace TikiShop.Core.Models.RequestModels.Identity
{
    public sealed class ChangePasswordRequest
    {
        [Required]
        public string NewPassword { get; set; }

        [Required]
        public string OldPassword { get; set; }
    }
}