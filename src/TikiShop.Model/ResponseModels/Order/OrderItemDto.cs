namespace TikiShop.Model.ResponseModels.Order;

public class OrderItemDto
{
    public string ProductId { get; set; }
    public string ProductName { get; set; }
    public int Units { get; set; }
    public double UnitPrice { get; set; }
    public string PictureUrl { get; set; }
}