using TikiShop.Core.RequestModels.Catalog;
using TikiShop.Core.ResponseModels;
using TikiShop.Core.ResponseModels.Catalog;

using PaginationRequest = TikiShop.Core.RequestModels.PaginationRequest;

namespace TikiShop.Core.Services.CatalogService.Queries;

public interface ICatalogQueries
{
    Task<PaginationResponse<GetListProductResponse>> GetListProducts(GetListProductRequest req);
    Task<GetProductByIdResponse> GetProductById(int id);
    Task<PaginationResponse<GetListBrandsResponse>> GetListBrands(PaginationRequest req);
    Task<List<GetAllCategoriesResponse>> GetCategoriesHierarchy();
    Task<PaginationResponse<GetAllCategoriesResponse>> GetCategories();
}