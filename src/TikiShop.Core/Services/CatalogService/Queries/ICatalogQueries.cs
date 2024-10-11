using TikiShop.Core.Models.RequestModels;
using TikiShop.Core.Models.RequestModels.Catalog;
using TikiShop.Core.Models.ResponseModels;
using TikiShop.Core.Models.ResponseModels.Catalog;

namespace TikiShop.Core.Services.CatalogService.Queries;

public interface ICatalogQueries
{
    Task<PaginationResponse<GetListProductResponse>> GetListProducts(GetListProductRequest req);
    Task<GetProductByIdResponse> GetProductById(int id);
    Task<PaginationResponse<GetListBrandsResponse>> GetListBrands(PaginationRequest req);
    Task<List<GetAllCategoriesResponse>> GetCategoriesHierarchy();
    Task<PaginationResponse<GetAllCategoriesResponse>> GetCategories();
}