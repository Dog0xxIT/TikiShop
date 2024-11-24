namespace TikiShop.Core.Services.OrderService.Queries;

public interface IOrderQueries
{
    Task<dynamic> GetOrderByBuyerId(int buyerId);
    Task<dynamic> CartTypes();
}