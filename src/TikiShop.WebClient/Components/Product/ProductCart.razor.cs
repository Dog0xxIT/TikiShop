using Microsoft.AspNetCore.Components;
using TikiShop.Shared.RequestModels.Basket;
using TikiShop.Shared.ResponseModels.Catalog;
using TikiShop.WebClient.Core;

namespace TikiShop.WebClient.Components.Product;

public partial class ProductCart
{
    [Parameter, EditorRequired] public GetListProductResponse ProductInfo { get; set; }

    [CascadingParameter(Name = "Notification")]
    private TelerikNotification _notificatioRef { get; set; }

    private async Task OnClickAddToCartButton(int productId)
    {
        var req = new UpdateBasketItemRequest
        {
            ProductId = productId,
            Quantity = 1,
        };
        var resultObject = await _basketService.UpdateBasketItem(req);
        if (resultObject.ResultCode.Equals(ResultCode.Success))
        {
            _notificatioRef.Show(
                text: resultObject.Messages,
                themeColor: ThemeConstants.Notification.ThemeColor.Light);
            return;
        }

        _notificatioRef.Show(
            text: resultObject.Messages,
            themeColor: ThemeConstants.Notification.ThemeColor.Error);
    }
}