using TikiShop.Shared.RequestModels.Basket;
using TikiShop.Shared.RequestModels.Catalog;
using TikiShop.Shared.ResponseModels.Basket;
using TikiShop.WebClient.Core;

namespace TikiShop.WebClient.Services.BasketService;

public interface IBasketService
{
    Task<GetBasketByCustomerIdResponse> GetBasketByCustomerId();
    Task<ResultObject> UpdateBasketItem(UpdateBasketItemRequest request);
    Task<ResultObject> DeleteBasketItem(int id);
}