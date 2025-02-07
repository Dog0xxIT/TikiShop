﻿using System.ComponentModel.DataAnnotations;

namespace TikiShop.Model.RequestModels.Identity;

public sealed class ChangePasswordReq
{
    [Required] [EmailAddress] public string Email { get; set; }

    [Required] public string NewPassword { get; set; }

    [Required] public string OldPassword { get; set; }
}