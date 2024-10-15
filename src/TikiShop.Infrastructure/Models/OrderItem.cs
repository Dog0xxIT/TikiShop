using TikiShop.Infrastructure.Common;

namespace TikiShop.Infrastructure.Models
{
    public class OrderItem : BaseEntity
    {
        public int ProductSkuId { get; set; }
        public double Discount { get; set; }
        public int Quantity { get; set; }
        public ProductSku ProductSku { get; set; }
    }
}
