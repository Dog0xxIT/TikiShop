using System.ComponentModel.DataAnnotations;

namespace TikiShop.Shared.RequestModels.Catalog
{
    public record CreateProductRequest
    {
        public string Sku { get; set; }

        [Required]
        public string Name { get; set; }

        public string? Description { get; set; }

        public string? ShortDescription { get; set; }

        [Range(0.001, double.MaxValue)]
        public double Price { get; set; }

        public string? ThumbnailUrl { get; set; }

        [Range(0, double.MaxValue)]
        public int Quantity { get; set; }

        public int CategoryId { get; set; }

        public int BrandId { get; set; }
    }
}
