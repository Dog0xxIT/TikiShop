using TikiShop.Core.Dto.Catalog;
using TikiShop.Core.Dto;
using TikiShop.Share.RequestModels;
using TikiShop.Share.RequestModels.Catalog;

namespace TikiShop.Core.Services.CatalogService.Queries;

public interface ICatalogQueries
{
    Task<PaginationDto<GetListProductDto>> GetListProducts(GetListProductRequest req);
    Task<GetProductByIdDto> GetProductById(int id);
    Task<PaginationDto<GetListBrandsDto>> GetListBrands(PaginationRequest req);
    Task<List<GetAllCategoriesDto>> GetCategoriesHierarchy();
    Task<PaginationDto<GetAllCategoriesDto>> GetCategories();
}