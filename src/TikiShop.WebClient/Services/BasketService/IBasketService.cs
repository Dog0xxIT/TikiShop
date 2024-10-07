using TikiShop.WebClient.Core;
using TikiShop.WebClient.Models.RequestModels.Basket;
using TikiShop.WebClient.Models.ResponseModels.Basket;
using TikiShop.WebClient.Models.ResponseModels.Common;

namespace TikiShop.WebClient.Services.BasketService;

public interface IBasketService
{
    Task<List<GetBasketByCustomerIdResponse>> GetBasketByCustomerId();
    Task<ResultObject<ResponseObject>> AddToBasket(AddToBasketRequest request);
    Task<ResultObject<ResponseObject>> UpdateQty(UpdateQtyRequest request);
    Task<ResultObject<ResponseObject>> DeleteBasketItem(int id);
}