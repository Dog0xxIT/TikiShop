using TikiShop.Model.Enums;

namespace TikiShop.Model.ResponseModels.Order;

public class GetOrdersByUserIdResp
{
    public string OrderId { get; set; }
    public DateTime Date { get; set; }
    public OrderStatus Status { get; set; }
    public double Total { get; set; }
}