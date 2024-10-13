using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace TikiShop.Shared.RequestModels.Catalog
{
    public class CreateBrandRequest
    {
        [Required]
        public string Name { get; set; }
    }
}
