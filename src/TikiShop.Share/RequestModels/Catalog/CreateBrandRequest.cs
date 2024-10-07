using System.ComponentModel.DataAnnotations;

namespace TikiShop.Share.RequestModels.Catalog
{
    public sealed class CreateBrandRequest
    {
        [Required]
        public string Name { get; set; }
    }
}
