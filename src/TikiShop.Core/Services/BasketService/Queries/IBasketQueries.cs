using TikiShop.Model.DTO;
using TikiShop.Model.ResponseModels.Basket;

namespace TikiShop.Core.Services.BasketService.Queries;

public interface IBasketQueries
{
    Task<ResultObject<GetBasketByCustomerIdResp>> GetBasketByCustomerId(int buyerId);
}