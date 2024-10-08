
using TikiShop.Infrastructure.Common;

namespace TikiShop.Core.Dto.Order
{
    public class GetOrdersByUserIdDto
    {
        public string OrderId { get; set; }
        public DateTime Date { get; set; }
        public OrderStatus Status { get; set; }
        public double Total { get; set; }
    }
}
