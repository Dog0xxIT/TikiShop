namespace TikiShop.Model.ResponseModels.Catalog;

public class GetProductByIdResp
{
    public int Id { get; set; }
    public string Name { get; set; }
    public double Price { get; set; }
    public double Discount { get; set; }
    public double RatingAverage { get; set; }
    public int ReviewCount { get; set; }
    public string ThumbnailUrl { get; set; }
    public int CategoryId { get; set; }
    public int BrandId { get; set; }
    public int TotalBought { get; set; }
    public string Sku { get; set; }
    public string ShortDescription { get; set; }
    public string Description { get; set; }
    public BrandDto Brand { get; set; } = new();
    public CategoryDto Category { get; set; } = new();
    public ConfigOptionDto? Option1 { get; set; }
    public ConfigOptionDto? Option2 { get; set; }
    public List<ProductSkuDto> ProductSkus { get; set; } = new();

    public class BrandDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
    }

    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ThumbnailUrl { get; set; }
        public int? ParentId { get; set; }
        public CategoryDto? Parent { get; set; }
        public bool? HasChild => Childs?.Any();
        public List<CategoryDto>? Childs { get; set; }
    }

    public class ConfigOptionDto
    {
        public string Type { get; set; }
        public string Code { get; set; }
        public string Value { get; set; }
    }

    public class ProductSkuDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Sku { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public ConfigOptionDto? Option1 { get; set; }
        public ConfigOptionDto? Option2 { get; set; }
    }
}