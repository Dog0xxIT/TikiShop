using System.ComponentModel.DataAnnotations;

namespace TikiShop.Model.RequestModels.Catalog;

public class CreateBrandRequest
{
    [Required] public string Name { get; set; }
}