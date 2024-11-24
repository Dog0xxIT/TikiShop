using TikiShop.Model.ResponseModels.Basket;

namespace TikiShop.Core.Services.BasketService.Queries;

public interface IBasketQueries
{
    Task<GetBasketByCustomerIdResponse> GetBasketByCustomerId(int buyerId);
}