using TikiShop.WebClient.Core;
using TikiShop.WebClient.Core.CoreHttpClient;
using TikiShop.WebClient.Models.RequestModels.Basket;
using TikiShop.WebClient.Models.ResponseModels.Basket;
using TikiShop.WebClient.Models.ResponseModels.Common;

namespace TikiShop.WebClient.Services.BasketService;

public class BasketService : IBasketService
{

    private readonly ICoreHttpClient _coreHttpClient;

    public BasketService(ICoreHttpClient coreHttpClient)
    {
        _coreHttpClient = coreHttpClient;
    }

    public async Task<List<GetBasketByCustomerIdResponse>> GetBasketByCustomerId()
    {
        var result = await _coreHttpClient.GetAsync<List<GetBasketByCustomerIdResponse>>(
            clientName: ClientsConfig.BasketClient,
            uri: "/api/v1/baskets/getBasketByCustomerId");
        return result.Data;
    }

    public async Task<ResultObject<ResponseObject>> AddToBasket(AddToBasketRequest request)
    {
        var result = await _coreHttpClient.PostAsync<ResponseObject>(
            clientName: ClientsConfig.BasketClient,
            uri: "api/v1/baskets/addToBasket",
            reqObj: request);
        return result;
    }

    public async Task<ResultObject<ResponseObject>> UpdateQty(UpdateQtyRequest request)
    {
        var result = await _coreHttpClient.PatchAsync<ResponseObject>(
            clientName: ClientsConfig.BasketClient,
            uri: "api/v1/baskets/updateQty",
            reqObj: request);
        return result;
    }

    public async Task<ResultObject<ResponseObject>> DeleteBasketItem(int id)
    {
        var result = await _coreHttpClient.DeleteAsync<ResponseObject>(
            clientName: ClientsConfig.BasketClient,
            uri: $"api/v1/baskets/{id}");
        return result;
    }
}