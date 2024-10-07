using Microsoft.AspNetCore.Components;
using TikiShop.WebClient.Core;
using TikiShop.WebClient.Models.RequestModels.Basket;
using TikiShop.WebClient.Models.ResponseModels.Catalog;

namespace TikiShop.WebClient.Components.Product;

public partial class ProductCart
{
    [Parameter, EditorRequired] public GetListProductResponse ProductInfo { get; set; }

    [CascadingParameter(Name = "Notification")]
    private TelerikNotification _notificatioRef { get; set; }

    private async Task OnClickAddToCartButton(int productId)
    {
        var req = new AddToBasketRequest
        {
            ProductId = productId,
            Quantity = 1,
        };
        var resultObject = await _basketService.AddToBasket(req);
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