using System.ComponentModel.DataAnnotations;

namespace TikiShop.Share.RequestModels.Identity
{
    public sealed class ChangePasswordRequest
    {
        [Required]
        public string NewPassword { get; set; }

        [Required]
        public string OldPassword { get; set; }
    }
}