﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TikiShop.Share.RequestModels.Identity
{
    public sealed class LoginRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [PasswordPropertyText]
        public string Password { get; set; }
    }
}