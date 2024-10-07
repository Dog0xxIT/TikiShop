using Catalog.Api.Data.Entities;

namespace TikiShop.Core.Models
{
    public class Product : BaseEntity
    {
        public string Sku { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? ShortDescription { get; set; }
        public double Price { get; set; }
        public string? ThumbnailUrl { get; set; }
        public int Quantity { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public int BrandId { get; set; }
        public Brand Brand { get; set; }
        public List<ProductVariant>? ProductVariant { get; set; }
        //[JsonIgnore]
        //public Vector Embedding { get; set; }
    }
}
