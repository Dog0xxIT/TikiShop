using System.ComponentModel.DataAnnotations.Schema;
using TikiShop.Infrastructure.Common;

namespace TikiShop.Infrastructure.Models
{
    public class Order : BaseEntity
    {
        [ForeignKey(nameof(Buyer))]
        public int BuyerId { get; set; }
        public int PaymentId { get; set; }
        public int AddressId { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public string Description { get; set; }

        public User Buyer { get; set; }
        public Address Address { get; set; }
        public PaymentMethod Payment { get; set; }
        public List<OrderItem> OrderItems { get; set; }
    }
}
