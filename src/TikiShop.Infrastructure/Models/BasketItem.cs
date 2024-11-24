namespace TikiShop.Infrastructure.Models;

public class BasketItem : BaseEntity
{
    public int Quantity { get; set; }
    public int ProductSkuId { get; set; }
    public ProductSku ProductSku { get; set; }
    public int BasketId { get; set; }
    public Basket Basket { get; set; }
}