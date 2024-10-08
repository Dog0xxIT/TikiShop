using TikiShop.Infrastructure.Common;

namespace TikiShop.Infrastructure.Models
{
    public class BasketItem : BaseEntity
    {
        public int Quantity { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int? ProductVariantId { get; set; }
        public ProductVariant? ProductVariant { get; set; }
        public int BasketId { get; set; }
        public Basket Basket { get; set; }
    }
}
