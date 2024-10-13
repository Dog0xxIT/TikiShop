using System.ComponentModel.DataAnnotations;

namespace TikiShop.Shared.RequestModels.Catalog;

public class UpdateBrandRequest
{
    [Required]
    public string Name { get; set; }
}