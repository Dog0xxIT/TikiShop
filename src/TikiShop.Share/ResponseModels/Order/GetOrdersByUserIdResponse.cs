
namespace TikiShop.Share.ResponseModels.Order
{
    public class GetOrdersByUserIdResponse
    {
        public string OrderId { get; set; }
        public DateTime Date { get; set; }
        public OrderStatus Status { get; set; }
        public double Total { get; set; }
    }
}
