using System.ComponentModel.DataAnnotations.Schema;

namespace TikiShop.Infrastructure.Models;

public class ProductSku : BaseEntity
{
    public int ProductId { get; set; }
    public Product Product { get; set; }
    public string Name { get; set; }
    public string Sku { get; set; }
    public double Price { get; set; }
    public int Quantity { get; set; }

    [ForeignKey(nameof(Attribute1))] public int? AttributeId1 { get; set; }

    public ProductAttribute? Attribute1 { get; set; }

    [ForeignKey(nameof(Attribute2))] public int? AttributeId2 { get; set; }

    public ProductAttribute? Attribute2 { get; set; }
}