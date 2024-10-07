using System.ComponentModel.DataAnnotations;

namespace TikiShop.Share.RequestModels.Catalog
{
    public sealed class UpdateBrandRequest
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
