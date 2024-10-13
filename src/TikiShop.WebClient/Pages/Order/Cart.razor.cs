using Microsoft.AspNetCore.Components;
using TikiShop.Shared.RequestModels.Basket;
using TikiShop.Shared.ResponseModels.Basket;
using TikiShop.WebClient.Core;

namespace TikiShop.WebClient.Pages.Order;

public partial class Cart
{
    [CascadingParameter(Name = "Notification")]
    private TelerikNotification _notificatioRef { get; set; }

    private GetBasketByCustomerIdResponse _basketResponse;
    private bool _visibleLoader;

    protected override async Task OnInitializedAsync()
    {
        _visibleLoader = true;
        _basketResponse = new();
        _basketResponse = await _basketService.GetBasketByCustomerId();
        _visibleLoader = false;
    }

    private async Task UpdateQty(int basketItemId, int qty)
    {
        var req = new UpdateBasketItemRequest()
        {
            Quantity = qty,
            ProductId = 0,
            ProductVariantId = 0,
        };
        _visibleLoader = true;
        var resultObject = await _basketService.UpdateBasketItem(req);
        if (resultObject.ResultCode.Equals(ResultCode.Success))
        {
            _basketResponse = await _basketService.GetBasketByCustomerId();
            _notificatioRef.Show(
                text: resultObject.Messages,
                themeColor: ThemeConstants.Notification.ThemeColor.Light);
            _visibleLoader = false;
            return;
        }
        _notificatioRef.Show(
            text: resultObject.Messages,
            themeColor: ThemeConstants.Notification.ThemeColor.Error);
        _visibleLoader = false;
    }

    private async Task DeleteBasketItem(int basketItemId)
    {
        _visibleLoader = true;
        var resultObject = await _basketService.DeleteBasketItem(basketItemId);
        if (resultObject.ResultCode.Equals(ResultCode.Success))
        {
            _basketResponse = await _basketService.GetBasketByCustomerId();
            _notificatioRef.Show(
                text: resultObject.Messages,
                themeColor: ThemeConstants.Notification.ThemeColor.Light);
            _visibleLoader = false;
            return;
        }
        _notificatioRef.Show(
            text: resultObject.Messages,
            themeColor: ThemeConstants.Notification.ThemeColor.Error);
        _visibleLoader = false;
    }

}