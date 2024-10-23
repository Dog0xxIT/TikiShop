using Mapster;
using Org.BouncyCastle.Ocsp;
using TikiShop.Infrastructure;
using TikiShop.Infrastructure.Models;
using TikiShop.Shared.Enums;
using TikiShop.Shared.RequestModels;
using TikiShop.Shared.RequestModels.Catalog;
using TikiShop.Shared.ResponseModels;
using TikiShop.Shared.ResponseModels.Catalog;

namespace TikiShop.Core.Services.CatalogService.Queries
{
    public class EfCatalogQueries : ICatalogQueries
    {
        private readonly TikiShopDbContext _context;
        private readonly ILogger<EfCatalogQueries> _logger;

        public EfCatalogQueries(TikiShopDbContext context, ILogger<EfCatalogQueries> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<PaginationResponse<ProductDto>> GetListProducts(GetListProductRequest req)
        {
            if (req.MinPrice > req.MaxPrice)
            {
                return new();
            }
            req.MinPrice ??= 0;
            req.MaxPrice ??= double.MaxValue;
            var brands = ParseIds(req.Brands);
            var categories = ParseIds(req.Categories);
            var queryable = _context.Products.AsQueryable().AsNoTracking();
            queryable = ApplyFilters(queryable, req, brands, categories);
            queryable = ApplySorting(queryable, req);
            var rnd = new Random();
            var productDtoList = await queryable
                .Select(product =>
                    new ProductDto
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Price = product.ProductSkus.First(ps => (ps.Price >= req.MinPrice && ps.Price <= req.MaxPrice)).Price,
                        Discount = rnd.Next(1, 100),
                        RatingAverage = rnd.Next(1, 5),
                        ReviewCount = rnd.Next(500, 20000),
                        ThumbnailUrl = product.ThumbnailUrl,
                        TotalBought = product.Id,
                        ShortDescription = product.Summary,
                        Description = product.Description,
                    })
                .Skip((req.Page - 1) * req.Limit)
                .Take(req.Limit)
                .ToListAsync();

            var totalProducts = await _context.Products.CountAsync();
            var totalPage = (totalProducts / req.Limit) + 1;

            var response = new PaginationResponse<ProductDto>
            {
                Data = productDtoList,
                Meta = new PaginationMetaDto
                {
                    Count = productDtoList.Count,
                    CurrentPage = req.Page,
                    Total = totalProducts,
                    TotalPages = totalPage,
                    PerPage = req.Limit
                }
            };

            return response;
        }

        public async Task<ProductDto> GetProductById(int id)
        {
            var rnd = new Random();
            var productDto = await _context.Products
                .AsNoTracking()
                .Select(product =>
                    new ProductDto
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
                        Brand = new BrandDto
                        {
                            Id = product.BrandId,
                            Name = product.Brand.Name,
                            Slug = product.Brand.Slug,
                        },
                        Category = new CategoryDto
                        {
                            Id = product.CategoryId,
                            Name = product.Category.Name,
                            ThumbnailUrl = product.Category.ThumbnailUrl,
                        },
                        ProductSkus = product.ProductSkus.Select(ps => new ProductSkuDto
                        {
                            Price = ps.Price,
                            Id = ps.Id,
                            Name = ps.Name,
                            Sku = ps.Sku,
                            Option1 = ps.AttributeId1 != null ? new ConfigOptionDto
                            {
                                Value = ps.Attribute1!.Value,
                                Code = ps.Attribute1.Code,
                                Type = ps.Attribute1.Type,
                            } : null,
                            Option2 = ps.AttributeId2 != null ? new ConfigOptionDto
                            {
                                Value = ps.Attribute2!.Value,
                                Code = ps.Attribute2.Code,
                                Type = ps.Attribute2.Type,
                            } : null,
                            Quantity = ps.Quantity
                        }).ToList(),
                    })
                .SingleOrDefaultAsync(product => product.Id == id);

            return productDto ?? new();
        }

        public async Task<PaginationResponse<BrandDto>> GetListBrands(PaginationRequest req)
        {
            var queryable = _context.Brands
                .Skip(req.Page)
                .Take(req.Limit);

            queryable = req.SortDescending ?
                queryable.OrderByDescending(p => p.Id) :
                queryable.OrderBy(p => p.Id);

            var brands = await queryable.ToListAsync();
            var totalBrand = await _context.Brands.CountAsync();
            var totalPage = (totalBrand / req.Limit) + 1;
            var brandsDto = brands.Adapt<List<BrandDto>>();

            var response = new PaginationResponse<BrandDto>
            {
                Data = brandsDto,
                Meta = new PaginationMetaDto
                {
                    Count = brandsDto.Count,
                    CurrentPage = req.Page,
                    Total = totalBrand,
                    TotalPages = totalPage,
                    PerPage = req.Limit
                }
            };

            return response;
        }


        public async Task<List<CategoryDto>> GetCategoriesHierarchy()
        {
            var categories = await _context.Categories
                .AsNoTracking()
                .ToListAsync();

            var categoriesDto = categories
                .Select(category => new CategoryDto
                {
                    Id = category.Id,
                    Name = category.Name,
                    ParentId = category.ParentId,
                    ThumbnailUrl = category.ThumbnailUrl,
                    Childs = GetChildCategories(category.Id, categories, 1),
                })
                .ToList();

            return categoriesDto;
        }

        public async Task<PaginationResponse<CategoryDto>> GetCategories(PaginationRequest req)
        {
            var queryable = _context.Categories
            .Skip(req.Page)
            .Take(req.Limit);

            queryable = req.SortDescending ?
                queryable.OrderByDescending(p => p.Id) :
                queryable.OrderBy(p => p.Id);

            var categories = await queryable.ToListAsync();
            var totalCategory = await _context.Categories.CountAsync();
            var totalPage = (totalCategory / req.Limit) + 1;

            var categoriesDto = categories.Select(category => new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                ParentId = category.ParentId,
                ThumbnailUrl = category.ThumbnailUrl,
                Childs = new(),
            }).ToList();

            var response = new PaginationResponse<CategoryDto>
            {
                Data = categoriesDto,
                Meta = new PaginationMetaDto
                {
                    Count = categoriesDto.Count,
                    CurrentPage = req.Page,
                    Total = categoriesDto.Count,
                    TotalPages = totalPage,
                    PerPage = req.Limit
                }
            };

            return response;
        }

        private IQueryable<Product> ApplySorting(IQueryable<Product> queryable, GetListProductRequest req)
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
            return string.IsNullOrEmpty(ids) ?
                new() : ids.Replace(" ", string.Empty).Split(",").Select(int.Parse).ToList();
        }
        private IQueryable<Product> ApplyFilters(IQueryable<Product> queryable,
            GetListProductRequest req,
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

        private List<CategoryDto> GetChildCategories(
            int parentCategoryId,
            List<Category> categoryList,
            int level)
        {
            if (level >= 3)
            {
                return new();
            }

            // Tìm danh mục con của danh mục hiện tại (parentCategory)
            var childCategories = categoryList.Where(c => c.ParentId == parentCategoryId);

            return childCategories.Select(child => new CategoryDto
            {
                Id = child.Id,
                Name = child.Name,
                ThumbnailUrl = child.ThumbnailUrl,
                ParentId = child.ParentId,
                Childs = GetChildCategories(child.Id, categoryList, level + 1) // Đệ quy tiếp tục cho các cấp thấp hơn
            }).ToList();
        }
    }
}
