namespace TikiShop.Infrastructure.Models;

public class Product : BaseEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Summary { get; set; }
    public string ThumbnailUrl { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; }
    public int BrandId { get; set; }
    public Brand Brand { get; set; }
    public List<ProductSku> ProductSkus { get; set; }

    //[JsonIgnore]
    //public Vector Embedding { get; set; }
}