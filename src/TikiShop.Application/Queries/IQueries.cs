using TikiShop.Share.RequestModels;
using TikiShop.Share.RequestModels.Catalog;
using TikiShop.Share.ResponseModels;
using TikiShop.Share.ResponseModels.Basket;
using TikiShop.Share.ResponseModels.Catalog;
using TikiShop.Share.ResponseModels.Order;

namespace TikiShop.Domain.Queries;

public interface IQueries
{
    Task<PaginationResponse<GetListBrandsResponse>> GetListBrands(PaginationRequest req);
    Task<List<GetAllCategoriesResponse>> GetAllCategoriesHierarchy();
    Task<PaginationResponse<GetAllCategoriesResponse>> GetAllCategories();
    Task<PaginationResponse<GetListProductResponse>> GetListProducts(GetListProductRequest req);
    Task<GetProductByIdResponse> GetProductById(int id);
    Task<List<GetBasketByCustomerIdResponse>> GetBasketByCustomerId(string userId);
    Task<List<GetCardTypesResponse>> GetCartTypes();
    Task<GetOrderResponse> GetOrderById();
    Task<List<GetOrdersByUserIdResponse>> GetOrdersByUserId();
}