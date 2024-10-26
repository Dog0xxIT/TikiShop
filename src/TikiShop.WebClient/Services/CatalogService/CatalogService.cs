using TikiShop.Shared.RequestModels;
using TikiShop.Shared.ResponseModels;
using TikiShop.Shared.ResponseModels.Catalog;
using TikiShop.WebClient.Core.CoreHttpClient;

namespace TikiShop.WebClient.Services.CatalogService
{
    public class CatalogService : ICatalogService
    {
        private readonly ICoreHttpClient _coreHttpClient;

        public CatalogService(ICoreHttpClient coreHttpClient)
        {
            _coreHttpClient = coreHttpClient;
        }

        public async Task<PaginationResponse<GetListBrandsResponse>> GetListBrands(PaginationRequest paginationRequest)
        {
            var resultObject = await _coreHttpClient.GetAsync<PaginationResponse<GetListBrandsResponse>>(
                clientName: "TikiShopApi",
                uri: "api/v1/catalog/brands",
                queryObj: paginationRequest);

            return resultObject.Data;
        }

        public async Task<List<GetListCategoriesResponse>> GetAllCategoriesHierarchy()
        {
            var resultObject = await _coreHttpClient.GetAsync<List<GetListCategoriesResponse>>(
                clientName: "TikiShopApi",
                uri: "api/v1/catalog/categories/hierarchy");

            return resultObject.Data;
        }

        public async Task<PaginationResponse<GetListCategoriesResponse>> GetAllCategories()
        {
            var resultObject = await _coreHttpClient.GetAsync<PaginationResponse<GetListCategoriesResponse>>(
                clientName: "TikiShopApi",
                uri: "api/v1/catalog/categories");

            return resultObject.Data;
        }

        public async Task<PaginationResponse<GetListProductsResponse>> GetListProducts(PaginationRequest paginationRequest)
        {
            var resultObject = await _coreHttpClient.GetAsync<PaginationResponse<GetListProductsResponse>>(
                 clientName: "TikiShopApi",
                 uri: "api/v1/catalog/products",
                 queryObj: paginationRequest);

            return resultObject.Data;
        }


        public async Task<GetProductByIdResponse> GetProductById(int productId)
        {
            var result = await _coreHttpClient.GetAsync<GetProductByIdResponse>(
                clientName: "TikiShopApi",
                uri: $"api/v1/catalog/products/{productId}");
            return result.Data;
        }
    }
}
