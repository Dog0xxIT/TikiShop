﻿namespace TikiShop.Model.ResponseModels.Identity;

public class ManageInfoResp
{
    public string Email { get; set; }
    public List<string> Roles { get; set; }
    public string UserId { get; set; }
}