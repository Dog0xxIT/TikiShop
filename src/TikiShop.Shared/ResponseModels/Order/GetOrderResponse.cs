using TikiShop.Shared.Enums;

namespace TikiShop.Shared.ResponseModels.Order
{
    public class GetOrderResponse
    {
        public string OrderId { get; set; }
        public DateTime Date { get; set; }
        public OrderStatus Status { get; set; }
        public string Description { get; set; }
        public double Total { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }
        public List<OrderItemDto> OrderItems { get; set; }
    }
}
