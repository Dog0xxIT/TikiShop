using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using TikiShop.Infrastructure;
using TikiShop.Infrastructure.Models;
using TikiShop.Share.RequestModels;
using TikiShop.Share.RequestModels.Catalog;
using TikiShop.Share.ResponseModels;
using TikiShop.Share.ResponseModels.Basket;
using TikiShop.Share.ResponseModels.Catalog;
using TikiShop.Share.ResponseModels.Order;
using Category = TikiShop.Domain.AggregatesModel.Category;

namespace TikiShop.Domain.Queries
{
    public class Queries : IQueries
    {
        private readonly TikiShopDapperContext _context;
        private readonly TikiShopDbContext _efContext;
        private readonly ILogger<Queries> _logger;

        public Queries(ILogger<Queries> logger, TikiShopDapperContext context, TikiShopDbContext efContext)
        {
            _logger = logger;
            _context = context;
            _efContext = efContext;
        }

        public async Task<PaginationResponse<GetListBrandsResponse>> GetListBrands(PaginationRequest req)
        {
            throw new NotImplementedException();
        }

        public async Task<List<GetAllCategoriesResponse>> GetAllCategoriesHierarchy()
        {
            throw new NotImplementedException();
        }

        public async Task<PaginationResponse<GetAllCategoriesResponse>> GetAllCategories()
        {
            var sql = "select * from categories";
            using var conn = _context.CreateConnection();
            var result = await conn.QueryAsync<Category>(sql);
            return new();
        }

        public async Task<PaginationResponse<GetListProductResponse>> GetListProducts(GetListProductRequest req)
        {
            throw new NotImplementedException();
        }

        public async Task<GetProductByIdResponse> GetProductById(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<GetBasketByCustomerIdResponse>> GetBasketByCustomerId(string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<GetCardTypesResponse>> GetCartTypes()
        {
            throw new NotImplementedException();
        }

        public async Task<GetOrderResponse> GetOrderById()
        {
            throw new NotImplementedException();
        }

        public async Task<List<GetOrdersByUserIdResponse>> GetOrdersByUserId()
        {
            throw new NotImplementedException();
        }
    }
}
