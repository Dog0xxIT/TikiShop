using System.ComponentModel.DataAnnotations;

namespace TikiShop.Model.RequestModels.Catalog;

public class UpdateBrandRequest
{
    [Required] public string Name { get; set; }
}