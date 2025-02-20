﻿using Microsoft.AspNetCore.Identity;

namespace TikiShop.Infrastructure.Models;

public class User : IdentityUser<int>
{
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
}