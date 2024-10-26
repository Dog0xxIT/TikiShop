using TikiShop.Shared.RequestModels;
using TikiShop.Shared.RequestModels.Catalog;
using TikiShop.Shared.ResponseModels;
using TikiShop.Shared.ResponseModels.Catalog;

namespace TikiShop.Core.Services.CatalogService.Queries;

public interface ICatalogQueries
{
    Task<PaginationResponse<GetListProductsResponse>> GetListProducts(GetListProductRequest req);
    Task<GetProductByIdResponse> GetProductById(int id);
    Task<PaginationResponse<GetListBrandsResponse>> GetListBrands(PaginationRequest req);
    Task<List<GetListCategoriesResponse>> GetCategoriesHierarchy();
    Task<PaginationResponse<GetListCategoriesResponse>> GetCategories( PaginationRequest req);
}