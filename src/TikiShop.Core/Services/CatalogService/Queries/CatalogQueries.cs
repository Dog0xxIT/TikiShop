using Dapper;
using TikiShop.Infrastructure;
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

        public async Task<PaginationResponse<GetListProductResponse>> GetListProducts(GetListProductRequest req)
        {
            using var conn = _dapperContext.CreateConnection();
            var sql = $"""
                       SELECT TOP ({req.Limit}) [Id], [Sku], [Name], [Description], [Summary], [Price], [ThumbnailUrl], [Quantity], [CategoryId], [Id], [Created], [LastModified], [IsDeleted], [TotalBuy] ToTalBuyer
                       FROM Products
                            LEFT JOIN (SELECT OI.BasketItemId, SUM(OI.Quantity) TotalBuy
                                FROM OrderItems OI
                              GROUP BY OI.BasketItemId) AS B ON Products.Id = B.BasketItemId
                       WHERE Products.IsDeleted = 0
                       ORDER BY Id;
                       SELECT TOP (100) [Id], [Sku], [Name], [Description], [Summary], [Price], [ThumbnailUrl], [Quantity], [CategoryId], [Id], [Created], [LastModified], [IsDeleted], [TotalBuy] ToTalBuyer
                       FROM Products
                            LEFT JOIN (SELECT OI.BasketItemId, SUM(OI.Quantity) TotalBuy
                                FROM OrderItems OI
                              GROUP BY OI.BasketItemId) AS B ON Products.Id = B.BasketItemId
                       WHERE Products.IsDeleted = 0
                       ORDER BY Id
                       """;
            if (req.SortDescending)
            {
                sql += "DESC";
            }
            var result = await conn.QueryAsync<GetListProductResponse>(sql);
            var total = await conn.ExecuteScalarAsync<int>("select count(*) from Products WHERE Products.IsDeleted = 0");
            var pagingResult = new PaginationResponse<GetListProductResponse>
            {
                Data = result,
                Meta = new()
                {
                    Count = result.Count(),
                    CurrentPage = req.Page,
                    PerPage = req.Limit,
                    Total = total,
                    TotalPages = total / req.Limit,
                }
            };
            return pagingResult;
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

        public async Task<List<GetAllCategoriesResponse>> GetCategoriesHierarchy()
        {
            var sql = $"""
                       select * from brands
                       """;
            _logger.LogInformation($"Execute command: {sql}");
            using var conn = _dapperContext.CreateConnection();
            var result = await conn.QueryAsync(sql);
            return new();
        }

        public async Task<PaginationResponse<GetAllCategoriesResponse>> GetCategories()
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
