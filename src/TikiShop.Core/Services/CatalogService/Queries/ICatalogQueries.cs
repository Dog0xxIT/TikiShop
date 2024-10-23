using TikiShop.Shared.RequestModels;
using TikiShop.Shared.RequestModels.Catalog;
using TikiShop.Shared.ResponseModels;
using TikiShop.Shared.ResponseModels.Catalog;

namespace TikiShop.Core.Services.CatalogService.Queries;

public interface ICatalogQueries
{
    Task<PaginationResponse<ProductDto>> GetListProducts(GetListProductRequest req);
    Task<ProductDto> GetProductById(int id);
    Task<PaginationResponse<BrandDto>> GetListBrands(PaginationRequest req);
    Task<List<CategoryDto>> GetCategoriesHierarchy();
    Task<PaginationResponse<CategoryDto>> GetCategories( PaginationRequest req);
}