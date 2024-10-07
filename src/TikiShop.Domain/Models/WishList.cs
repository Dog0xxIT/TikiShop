using Catalog.Api.Data.Entities;

namespace TikiShop.Core.Models
{
    public class WishList : BaseEntity
    {
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int UserId { get; set; }
        public User Buyer { get; set; }
    }
}
