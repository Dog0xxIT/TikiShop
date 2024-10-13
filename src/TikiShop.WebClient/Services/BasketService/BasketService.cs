using TikiShop.Shared.RequestModels.Basket;
using TikiShop.Shared.ResponseModels.Basket;
using TikiShop.WebClient.Core;
using TikiShop.WebClient.Core.CoreHttpClient;

namespace TikiShop.WebClient.Services.BasketService;

public class BasketService : IBasketService
{

    private readonly ICoreHttpClient _coreHttpClient;

    public BasketService(ICoreHttpClient coreHttpClient)
    {
        _coreHttpClient = coreHttpClient;
    }

    public async Task<GetBasketByCustomerIdResponse> GetBasketByCustomerId()
    {
        var result = await _coreHttpClient.GetAsync<GetBasketByCustomerIdResponse>(
            clientName: "TikiShopApi",
            uri: "/api/v1/baskets");
        return result.Data;
    }

    public async Task<ResultObject> UpdateBasketItem(UpdateBasketItemRequest request)
    {
        var result = await _coreHttpClient.PatchAsync(
            clientName: "TikiShopApi",
            uri: "api/v1/baskets",
            reqObj: request);
        return result;
    }

    public async Task<ResultObject> DeleteBasketItem(int id)
    {
        var result = await _coreHttpClient.DeleteAsync(
            clientName: "TikiShopApi",
            uri: $"api/v1/baskets/{id}");
        return result;
    }
}