using System.ComponentModel.DataAnnotations;

namespace TikiShop.Model.RequestModels.Catalog;

public class CreateBrandReq
{
    [Required] public string Name { get; set; }
}