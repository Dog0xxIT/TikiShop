using Dapper;
using Microsoft.Extensions.Logging;
using TikiShop.Core.Dto;
using TikiShop.Core.Dto.Catalog;
using TikiShop.Infrastructure;
using TikiShop.Share.RequestModels;
using TikiShop.Share.RequestModels.Catalog;

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

        public async Task<PaginationDto<GetListProductDto>> GetListProducts(GetListProductRequest req)
        {
            using var conn = _dapperContext.CreateConnection();
            var sql = $"""
                       SELECT TOP ({req.Limit}) [Id], [Sku], [Name], [Description], [ShortDescription], [Price], [ThumbnailUrl], [Quantity], [CategoryId], [BrandId], [Created], [LastModified], [IsDeleted], [TotalBuy] ToTalBuyer
                       FROM Products
                            LEFT JOIN (SELECT OI.ProductId, SUM(OI.Units) TotalBuy
                                FROM OrderItems OI
                              GROUP BY OI.ProductId) AS B ON Products.Id = B.ProductId
                       WHERE Products.IsDeleted = 0
                       ORDER BY Id;
                       SELECT TOP (100) [Id], [Sku], [Name], [Description], [ShortDescription], [Price], [ThumbnailUrl], [Quantity], [CategoryId], [BrandId], [Created], [LastModified], [IsDeleted], [TotalBuy] ToTalBuyer
                       FROM Products
                            LEFT JOIN (SELECT OI.ProductId, SUM(OI.Units) TotalBuy
                                FROM OrderItems OI
                              GROUP BY OI.ProductId) AS B ON Products.Id = B.ProductId
                       WHERE Products.IsDeleted = 0
                       ORDER BY Id
                       """;
            if (req.SortDescending)
            {
                sql += "DESC";
            }
            var result = await conn.QueryAsync<GetListProductDto>(sql);
            var total = await conn.ExecuteScalarAsync<int>("select count(*) from Products WHERE Products.IsDeleted = 0");
            var pagingResult = new PaginationDto<GetListProductDto>
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

        public async Task<GetProductByIdDto> GetProductById(int id)
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

        public async Task<PaginationDto<GetListBrandsDto>> GetListBrands(PaginationRequest req)
        {
            var sql = $"""
                       select * from brands
                       """;
            _logger.LogInformation($"Execute command: {sql}");
            using var conn = _dapperContext.CreateConnection();
            var result = await conn.QueryAsync(sql);
            return new();
        }

        public async Task<List<GetAllCategoriesDto>> GetCategoriesHierarchy()
        {
            var sql = $"""
                       select * from brands
                       """;
            _logger.LogInformation($"Execute command: {sql}");
            using var conn = _dapperContext.CreateConnection();
            var result = await conn.QueryAsync(sql);
            return new();
        }

        public async Task<PaginationDto<GetAllCategoriesDto>> GetCategories()
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
