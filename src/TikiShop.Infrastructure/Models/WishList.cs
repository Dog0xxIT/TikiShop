using TikiShop.Infrastructure.Common;

namespace TikiShop.Infrastructure.Models
{
    public class WishList : BaseEntity
    {
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
