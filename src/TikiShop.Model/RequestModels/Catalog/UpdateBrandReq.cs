﻿using System.ComponentModel.DataAnnotations;

namespace TikiShop.Model.RequestModels.Catalog;

public class UpdateBrandReq
{
    [Required] public string Name { get; set; }
}