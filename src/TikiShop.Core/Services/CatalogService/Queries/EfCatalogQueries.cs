using Mapster;
using TikiShop.Model.DTO;
using TikiShop.Model.Enums;
using TikiShop.Model.RequestModels;
using TikiShop.Model.RequestModels.Catalog;
using TikiShop.Model.ResponseModels;
using TikiShop.Model.ResponseModels.Catalog;

namespace TikiShop.Core.Services.CatalogService.Queries;

public class EfCatalogQueries : ICatalogQueries
{
    private readonly TikiShopDbContext _context;
    private readonly ILogger<EfCatalogQueries> _logger;

    public EfCatalogQueries(TikiShopDbContext context, ILogger<EfCatalogQueries> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ResultObject<PaginationResp<GetListProductsResp>>> GetListProducts(GetListProductReq req)
    {
        if (req.MinPrice > req.MaxPrice)
        {
            _logger.LogWarning($"Invalid price range: MinPrice: {req.MinPrice}, MaxPrice: {req.MaxPrice}");
            return new ResultObject<PaginationResp<GetListProductsResp>>();
        }

        req.MinPrice ??= 0;
        req.MaxPrice ??= double.MaxValue;
        var brands = ParseIds(req.Brands);
        var categories = ParseIds(req.Categories);
        var queryable = _context.Products.AsQueryable().AsNoTracking();
        queryable = ApplyFilters(queryable, req, brands, categories);
        queryable = ApplySorting(queryable, req);
        var rnd = new Random();

        _logger.LogInformation($"Retrieving product list for page {req.Page} with limit {req.Limit}");

        var productDtoList = await queryable
            .Select(product =>
                new GetListProductsResp
                {
                    Id = product.Id,
                    Name = product.Name,
                    Price = product.ProductSkus.First(ps => ps.Price >= req.MinPrice && ps.Price <= req.MaxPrice).Price,
                    Discount = rnd.Next(1, 100),
                    RatingAverage = rnd.Next(1, 5),
                    ReviewCount = rnd.Next(500, 20000),
                    ThumbnailUrl = product.ThumbnailUrl,
                    TotalBought = product.Id,
                    ShortDescription = product.Summary,
                    Description = product.Description
                })
            .Skip((req.Page - 1) * req.Limit)
            .Take(req.Limit)
            .ToListAsync();

        var totalProducts = await _context.Products.CountAsync();
        var totalPage = totalProducts / req.Limit + 1;

        var response = new PaginationResp<GetListProductsResp>
        {
            Data = productDtoList,
            Meta = new PaginationMeta
            {
                Count = productDtoList.Count,
                CurrentPage = req.Page,
                Total = totalProducts,
                TotalPages = totalPage,
                PerPage = req.Limit
            }
        };

        _logger.LogInformation($"Successfully retrieved {productDtoList.Count} products for page {req.Page}");
        return ResultObject<PaginationResp<GetListProductsResp>>.Success(response);
    }

    public async Task<ResultObject<GetProductByIdResp>> GetProductById(int id)
    {
        var rnd = new Random();
        _logger.LogInformation($"Retrieving product details for ProductId: {id}");

        var productDto = await _context.Products
            .AsNoTracking()
            .Select(product =>
                new GetProductByIdResp
                {
                    Id = product.Id,
                    Name = product.Name,
                    Discount = rnd.Next(1, 100),
                    RatingAverage = rnd.Next(1, 5),
                    ReviewCount = rnd.Next(500, 20000),
                    ThumbnailUrl = product.ThumbnailUrl,
                    TotalBought = product.Id,
                    ShortDescription = product.Summary,
                    Description = product.Description,
                    Brand = new GetProductByIdResp.BrandDto
                    {
                        Id = product.BrandId,
                        Name = product.Brand.Name,
                        Slug = product.Brand.Slug
                    },
                    Category = new GetProductByIdResp.CategoryDto
                    {
                        Id = product.CategoryId,
                        Name = product.Category.Name,
                        ThumbnailUrl = product.Category.ThumbnailUrl
                    },
                    ProductSkus = product.ProductSkus.Select(ps => new GetProductByIdResp.ProductSkuDto
                    {
                        Price = ps.Price,
                        Id = ps.Id,
                        Name = ps.Name,
                        Sku = ps.Sku,
                        Option1 = ps.AttributeId1 != null
                            ? new GetProductByIdResp.ConfigOptionDto
                            {
                                Value = ps.Attribute1!.Value,
                                Code = ps.Attribute1.Code,
                                Type = ps.Attribute1.Type
                            }
                            : null,
                        Option2 = ps.AttributeId2 != null
                            ? new GetProductByIdResp.ConfigOptionDto
                            {
                                Value = ps.Attribute2!.Value,
                                Code = ps.Attribute2.Code,
                                Type = ps.Attribute2.Type
                            }
                            : null,
                        Quantity = ps.Quantity
                    }).ToList()
                })
            .SingleOrDefaultAsync(product => product.Id == id);

        if (productDto != null)
            _logger.LogInformation($"Successfully retrieved product details for ProductId: {id}");
        else
            _logger.LogWarning($"No product found for ProductId: {id}");

        return ResultObject<GetProductByIdResp>.Success(productDto);
    }

    public async Task<ResultObject<PaginationResp<GetListBrandsResp>>> GetListBrands(PaginationReq req)
    {
        _logger.LogInformation($"Retrieving brand list for page {req.Page} with limit {req.Limit}");

        var queryable = _context.Brands
            .Skip(req.Page)
            .Take(req.Limit);

        queryable = req.SortDescending ? queryable.OrderByDescending(p => p.Id) : queryable.OrderBy(p => p.Id);

        var brands = await queryable.ToListAsync();
        var totalBrand = await _context.Brands.CountAsync();
        var totalPage = totalBrand / req.Limit + 1;
        var brandsDto = brands.Adapt<List<GetListBrandsResp>>();

        var response = new PaginationResp<GetListBrandsResp>
        {
            Data = brandsDto,
            Meta = new PaginationMeta
            {
                Count = brandsDto.Count,
                CurrentPage = req.Page,
                Total = totalBrand,
                TotalPages = totalPage,
                PerPage = req.Limit
            }
        };

        _logger.LogInformation($"Successfully retrieved {brandsDto.Count} brands for page {req.Page}");
        return ResultObject<PaginationResp<GetListBrandsResp>>.Success(response);
    }

    public async Task<ResultObject<List<GetListCategoriesResp>>> GetCategoriesHierarchy()
    {
        _logger.LogInformation("Retrieving categories hierarchy");

        var categories = await _context.Categories
            .AsNoTracking()
            .ToListAsync();

        var categoriesDto = categories
            .Where(category => category.ParentId == null)
            .Select(category => new GetListCategoriesResp
            {
                Id = category.Id,
                Name = category.Name,
                ParentId = category.ParentId,
                ThumbnailUrl = category.ThumbnailUrl,
                Child = GetChildCategories(category.Id, categories, 1)
            })
            .ToList();

        _logger.LogInformation($"Successfully retrieved {categoriesDto.Count} categories");

        return ResultObject<List<GetListCategoriesResp>>.Success(categoriesDto);
    }

    public async Task<ResultObject<PaginationResp<GetListCategoriesResp>>> GetCategories(PaginationReq req)
    {
        _logger.LogInformation($"Retrieving categories for page {req.Page} with limit {req.Limit}");

        var queryable = _context.Categories
            .Skip(req.Page)
            .Take(req.Limit);

        queryable = req.SortDescending ? queryable.OrderByDescending(p => p.Id) : queryable.OrderBy(p => p.Id);

        var categories = await queryable.ToListAsync();
        var totalCategory = await _context.Categories.CountAsync();
        var totalPage = totalCategory / req.Limit + 1;

        var categoriesDto = categories.Select(category => new GetListCategoriesResp
        {
            Id = category.Id,
            Name = category.Name,
            ParentId = category.ParentId,
            ThumbnailUrl = category.ThumbnailUrl,
            Child = new List<GetListCategoriesResp>()
        }).ToList();

        var response = new PaginationResp<GetListCategoriesResp>
        {
            Data = categoriesDto,
            Meta = new PaginationMeta
            {
                Count = categoriesDto.Count,
                CurrentPage = req.Page,
                Total = categoriesDto.Count,
                TotalPages = totalPage,
                PerPage = req.Limit
            }
        };

        _logger.LogInformation($"Successfully retrieved {categoriesDto.Count} categories for page {req.Page}");

        return ResultObject<PaginationResp<GetListCategoriesResp>>.Success(response);
    }

    private IQueryable<Product> ApplySorting(IQueryable<Product> queryable, GetListProductReq req)
    {
        queryable = req.SortBy switch
        {
            SortBy.Id => queryable.OrderBy(product => product.Id),
            SortBy.Price => queryable.OrderBy(product => product.Id),
            SortBy.DateModified => queryable.OrderBy(product => product.Id),
            _ => queryable
        };

        return req.SortDescending ? queryable.OrderByDescending(product => product.LastModified) : queryable;
    }

    private List<int> ParseIds(string? ids)
    {
        return string.IsNullOrEmpty(ids)
            ? new List<int>()
            : ids.Replace(" ", string.Empty).Split(",").Select(int.Parse).ToList();
    }

    private IQueryable<Product> ApplyFilters(IQueryable<Product> queryable,
        GetListProductReq req,
        List<int> brands,
        List<int> categories)
    {
        if (req.MinPrice != null)
            queryable = queryable.Where(product => product.ProductSkus.Any(ps => ps.Price >= req.MinPrice));

        if (req.MaxPrice != null)
            queryable = queryable.Where(product => product.ProductSkus.Any(ps => ps.Price <= req.MaxPrice));

        if (brands.Any())
            queryable = queryable.Where(product => brands.Contains(product.BrandId));

        if (categories.Any())
            queryable = queryable.Where(product => categories.Contains(product.CategoryId));

        if (!string.IsNullOrEmpty(req.Keyword))
            queryable = queryable.Where(product => product.Name.Contains(req.Keyword));

        return queryable;
    }

    private List<GetListCategoriesResp> GetChildCategories(
        int parentCategoryId,
        List<Category> categoryList,
        int level)
    {
        if (level >= 3) return new List<GetListCategoriesResp>();

        var childCategories = categoryList.Where(c => c.ParentId == parentCategoryId);

        return childCategories.Select(child => new GetListCategoriesResp
        {
            Id = child.Id,
            Name = child.Name,
            ThumbnailUrl = child.ThumbnailUrl,
            ParentId = child.ParentId,
            Child = GetChildCategories(child.Id, categoryList, level + 1)
        }).ToList();
    }
}