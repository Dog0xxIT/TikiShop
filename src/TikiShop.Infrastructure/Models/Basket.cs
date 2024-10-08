using TikiShop.Infrastructure.Common;

namespace TikiShop.Infrastructure.Models
{
    public class Basket : BaseEntity
    {
        public int BuyerId { get; set; }
        public User Buyer { get; set; }
        public double Total { get; set; }
        public List<BasketItem> Items { get; set; }
    }
}
