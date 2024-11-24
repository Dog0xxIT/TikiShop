namespace TikiShop.Infrastructure.Models;

public class ProductAttribute : BaseEntity
{
    public string Type { get; set; }
    public string Value { get; set; }
    public string Code { get; set; }
}