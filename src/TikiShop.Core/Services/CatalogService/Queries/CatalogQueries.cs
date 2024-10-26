using Dapper;
using TikiShop.Infrastructure;
using TikiShop.Infrastructure.Models;
using TikiShop.Shared.RequestModels;
using TikiShop.Shared.RequestModels.Catalog;
using TikiShop.Shared.ResponseModels;
using TikiShop.Shared.ResponseModels.Catalog;

namespace TikiShop.Core.Services.CatalogService.Queries
{
    public class CatalogQueries : ICatalogQueries
    {
        private readonly TikiShopDapperContext _dapperContext;
        private readonly ILogger<CatalogQueries> _logger;

        public CatalogQueries(
            TikiShopDapperContext dapperContext,
            ILogger<CatalogQueries> logger,
            TikiShopDbContext efDbContext)
        {
            _dapperContext = dapperContext;
            _logger = logger;
        }

        public async Task<PaginationResponse<GetListProductsResponse>> GetListProducts(GetListProductRequest req)
        {
            using var conn = _dapperContext.CreateConnection();
            var sql = $"""
                      SELECT TOP {req.Limit} P.Id, P.Name, P.ThumbnailUrl, P.Summary AS ShortDescription, P.Description, P.CategoryId, P.BrandId, 
                      FROM Products AS P
                      WHERE P.IsDeleted = 0
                      ORDER BY P.Id DESC;
                      """;
            var products = await conn.QueryAsync<Product>(sql);
            foreach (var product in products)
            {
                sql = $"""
                       SELECT PS.Id, PS.ProductId, PS.Name, PS.Sku, PS.Price, PS.Quantity, PS.AttributeId1, PS.AttributeId2, PS.Created, PS.LastModified, PS.IsDeleted
                       FROM ProductSkus AS PS LEFT JOIN ProductAttribute AS PA ON PS.AttributeId1 = PA.Id OR PS.AttributeId2 = PA.Id 
                       """;
            }
            products.ToList().ForEach(product => Console.WriteLine($"Product: {product.Name}, Category: {product.Category.Name}"));

            Console.ReadLine();
            return new();
        }

        public async Task<GetProductByIdResponse> GetProductById(int id)
        {
            var sql = $"""
                       select * from products
                       where id = {id}
                       """;
            _logger.LogInformation($"Execute command: {sql}");
            using var conn = _dapperContext.CreateConnection();
            var result = await conn.QueryAsync(sql);
            return new();
        }

        public async Task<PaginationResponse<GetListBrandsResponse>> GetListBrands(PaginationRequest req)
        {
            var sql = $"""
                       select * from brands
                       """;
            _logger.LogInformation($"Execute command: {sql}");
            using var conn = _dapperContext.CreateConnection();
            var result = await conn.QueryAsync(sql);
            return new();
        }

        public async Task<List<GetListCategoriesResponse>> GetCategoriesHierarchy()
        {
            var sql = $"""
                       select * from brands
                       """;
            _logger.LogInformation($"Execute command: {sql}");
            using var conn = _dapperContext.CreateConnection();
            var result = await conn.QueryAsync(sql);
            return new();
        }

        public async Task<PaginationResponse<GetListCategoriesResponse>> GetCategories(PaginationRequest req)
        {
            var sql = $"""
                       select * from categories
                       """;
            _logger.LogInformation($"Execute command: {sql}");
            using var conn = _dapperContext.CreateConnection();
            var result = await conn.QueryAsync(sql);
            return new();
        }
    }
}
