using TikiShop.Model.DTO;
using TikiShop.Model.RequestModels;
using TikiShop.Model.RequestModels.Catalog;
using TikiShop.Model.ResponseModels;
using TikiShop.Model.ResponseModels.Catalog;

namespace TikiShop.Core.Services.CatalogService.Queries;

public interface ICatalogQueries
{
    Task<ResultObject<PaginationResp<GetListProductsResp>>> GetListProducts(GetListProductReq req);
    Task<ResultObject<GetProductByIdResp>> GetProductById(int id);
    Task<ResultObject<PaginationResp<GetListBrandsResp>>> GetListBrands(PaginationReq req);
    Task<ResultObject<List<GetListCategoriesResp>>> GetCategoriesHierarchy();
    Task<ResultObject<PaginationResp<GetListCategoriesResp>>> GetCategories(PaginationReq req);
}