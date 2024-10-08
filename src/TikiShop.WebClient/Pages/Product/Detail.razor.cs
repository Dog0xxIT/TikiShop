using Microsoft.AspNetCore.Components;
using TikiShop.WebClient.Models.ResponseModels.Catalog;
using TikiShop.WebClient.Models.ViewModels;

namespace TikiShop.WebClient.Pages.Product;

public partial class Detail
{
    [Parameter]
    public int ProductId { get; set; }

    private GetProductByIdResponse _productInfo;
    private List<BreadcrumbItem> _breadcrumbItems;
    private int _quantity;
    protected override async Task OnInitializedAsync()
    {
        
        _productInfo = new()
        {
            Brand = new(),
            Category = new(),
        };

        var resultObject = await CatalogService.GetProductById(ProductId);

        _productInfo = resultObject.Data;

        _breadcrumbItems = new List<BreadcrumbItem>
        {
            new () { Text = "Home", Icon = SvgIcon.Home },
            new ()  { Text = "Products"},
            new ()  { Text = @_productInfo.Name},
        };
    }
}
