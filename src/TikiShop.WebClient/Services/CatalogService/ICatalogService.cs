using TikiShop.Shared.RequestModels;
using TikiShop.Shared.ResponseModels;
using TikiShop.Shared.ResponseModels.Catalog;
using TikiShop.WebClient.Core;

namespace TikiShop.WebClient.Services.CatalogService;

public interface ICatalogService
{
    Task<PaginationResponse<GetListProductResponse>> GetListProducts(PaginationRequest paginationRequest);
    Task<GetProductByIdResponse> GetProductById(int productId);
    Task<PaginationResponse<GetListBrandsResponse>> GetListBrands(PaginationRequest paginationRequest);
    Task<List<GetAllCategoriesResponse>> GetAllCategoriesHierarchy();
    Task<PaginationResponse<GetAllCategoriesResponse>> GetAllCategories();
}