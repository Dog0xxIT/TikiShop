using TikiShop.WebClient.Core;
using TikiShop.WebClient.Models.RequestModels.Common;
using TikiShop.WebClient.Models.ResponseModels.Catalog;
using TikiShop.WebClient.Models.ResponseModels.Common;

namespace TikiShop.WebClient.Services.CatalogService;

public interface ICatalogService
{
    Task<PaginationResponse<GetListBrandsResponse>> GetListBrands(PaginationRequest paginationRequest);
    Task<List<GetAllCategoriesResponse>> GetAllCategoriesHierarchy();
    Task<PaginationResponse<GetAllCategoriesResponse>> GetAllCategories();
    Task<PaginationResponse<GetListProductResponse>> GetListProducts(PaginationRequest paginationRequest);
    Task<ResultObject<GetProductByIdResponse>> GetProductById(int productId);
}