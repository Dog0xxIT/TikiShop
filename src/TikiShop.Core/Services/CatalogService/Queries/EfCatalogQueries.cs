using Microsoft.Extensions.Logging;
using Mapster;
using Microsoft.EntityFrameworkCore;
using TikiShop.Core.Enums;
using TikiShop.Core.Models.RequestModels;
using TikiShop.Infrastructure;
using TikiShop.Infrastructure.Models;
using TikiShop.Core.Models.ResponseModels;
using TikiShop.Core.Models.RequestModels.Catalog;
using TikiShop.Core.Models.ResponseModels.Catalog;
using TikiShop.Infrastructure.Common;

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

        public async Task<PaginationResponse<GetListProductResponse>> GetListProducts(GetListProductRequest req)
        {
            if (req.MinPrice > req.MaxPrice)
            {
                return new();
            }

            var brands = ParseIds(req.Brands);
            var categories = ParseIds(req.Categories);

            var queryable = _context.Products.AsQueryable().AsNoTracking();
            queryable = ApplyFilters(queryable, req, brands, categories);
            queryable = ApplySorting(queryable, req);

            var products = await queryable
                .Skip((req.Page - 1) * req.Limit)
                .Take(req.Limit)
                .ToListAsync();

            var productsDto = products.Select(product =>
                new GetListProductResponse
                {
                    Id = product.Id,
                    CategoryId = product.CategoryId,
                    Price = product.Price,
                    BrandId = product.BrandId,
                    //Discount = product.Discount,
                    Name = product.Name,
                    ThumbnailUrl = product.ThumbnailUrl,
                    //ReviewCount = product.ReviewCount,
                    //RatingAverage = product.RatingAverage,
                    // TotalBought = product.TotalBought,
                    Sku = product.Sku,
                    ShortDescription = product.ShortDescription,
                }).ToList();

            var totalProducts = await _context.Products.CountAsync();
            var totalPage = req.Limit != 0 ? (totalProducts / req.Limit) + 1 : 0;

            var response = new PaginationResponse<GetListProductResponse>
            {
                Data = productsDto,
                Meta = new PaginationMetaDto
                {
                    Count = productsDto.Count,
                    CurrentPage = req.Page,
                    Total = totalProducts,
                    TotalPages = totalPage,
                    PerPage = req.Limit
                }
            };

            return response;
        }

        public async Task<GetProductByIdResponse> GetProductById(int id)
        {
            var product = await _context.Products
                .AsNoTracking()
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(product => product.ProductVariant)
                .SingleOrDefaultAsync(p => p.Id == id);

            if (product is null)
            {
                return new();
            }

            var configOptions = new List<GetProductByIdResponse.ConfigOption>();

            if (product.ProductVariant != null)
            {
                var optionTypeId1 = product.ProductVariant.FirstOrDefault()?.OptionTypeId1;
                var optionTypeId2 = product.ProductVariant.FirstOrDefault()?.OptionTypeId2;

                var options1 = await _context.OptionTypes
                    .AsNoTracking()
                    .SingleOrDefaultAsync(i => i.Id == optionTypeId1);
                if (options1 != null)
                {
                    configOptions.Add(new GetProductByIdResponse.ConfigOption
                    {
                        OptionCode = options1.Code,
                        OptionName = options1.Name,
                        Values = product.ProductVariant.Select(pv => pv.OptionValue1).ToList(),
                    });
                }

                var options2 = await _context.OptionTypes
                    .AsNoTracking()
                    .SingleOrDefaultAsync(i => i.Id == optionTypeId2);
                if (options2 != null)
                {
                    configOptions.Add(new GetProductByIdResponse.ConfigOption
                    {
                        OptionCode = options2.Code,
                        OptionName = options2.Name,
                        Values = product.ProductVariant.Select(pv => pv.OptionValue2).ToList()!,
                    });
                }
            }


            var response = new GetProductByIdResponse
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description ?? "",
                Price = product.Price,
                ThumbnailUrl = product.ThumbnailUrl ?? "",
                //Discount = product.Discount,
                //ReviewCount = product.ReviewCount,
                //RatingAverage = product.RatingAverage,
                //TotalBought = product.TotalBought,
                Sku = product.Sku,
                ShortDescription = product.ShortDescription ?? "",
                Brand = new()
                {
                    Id = product.BrandId,
                    Name = product?.Brand.Name ?? "",
                    Slug = product?.Brand.Slug ?? ""
                },
                Category = new()
                {
                    Id = product!.CategoryId,
                    Name = product?.Category.Name ?? "",
                    ThumbnailUrl = product?.Category.ThumbnailUrl ?? "",
                },
                Variants = product!.ProductVariant
                    ?.Select(pv => new GetProductByIdResponse.Variant
                    {
                        ProductId = product.Id,
                        Price = pv.Price,
                        AvailableStock = pv.Quantity,
                        Option1 = pv.OptionValue1,
                        Option2 = pv.OptionValue2
                    })
                    ?.ToList() ?? new(),
                ConfigOptions = configOptions
            };

            return response;
        }

        public async Task<PaginationResponse<GetListBrandsResponse>> GetListBrands(PaginationRequest req)
        {
            var queryable = _context.Brands
                .Skip(req.Page)
                .Take(req.Limit);

            queryable = req.SortDescending ?
                queryable.OrderByDescending(p => p.Id) :
                queryable.OrderBy(p => p.Id);

            var brands = await queryable.ToListAsync();
            var totalBrand = await _context.Brands.CountAsync();
            var totalPage = req.Limit != 0 ?
                (totalBrand / req.Limit) + 1 : 0;
            var brandsDto = brands.Adapt<List<GetListBrandsResponse>>();

            var response = new PaginationResponse<GetListBrandsResponse>
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


        public async Task<List<GetAllCategoriesResponse>> GetCategoriesHierarchy()
        {
            var categories = await _context.Categories
                .AsNoTracking()
                .ToListAsync();

            var categoriesDto = categories
                .Select(category => new GetAllCategoriesResponse
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

        public async Task<PaginationResponse<GetAllCategoriesResponse>> GetCategories()
        {
            var categories = await _context.Categories.ToListAsync();

            var categoriesDto = categories.Select(category => new GetAllCategoriesResponse
            {
                Id = category.Id,
                Name = category.Name,
                ParentId = category.ParentId,
                ThumbnailUrl = category.ThumbnailUrl,
                Childs = new(),
            }).ToList();

            var response = new PaginationResponse<GetAllCategoriesResponse>
            {
                Data = categoriesDto,
                Meta = new PaginationMetaDto
                {
                    Count = categoriesDto.Count,
                    CurrentPage = 1,
                    Total = categoriesDto.Count,
                    TotalPages = 1,
                    PerPage = 1
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
                queryable = queryable.Where(product => product.Price >= req.MinPrice);

            if (req.MaxPrice != null)
                queryable = queryable.Where(product => product.Price <= req.MaxPrice);

            if (brands.Any())
                queryable = queryable.Where(product => brands.Contains(product.BrandId));

            if (categories.Any())
                queryable = queryable.Where(product => categories.Contains(product.CategoryId));

            if (!string.IsNullOrEmpty(req.Keyword))
                queryable = queryable.Where(product => product.Name.Contains(req.Keyword));

            return queryable;
        }

        private List<GetAllCategoriesResponse> GetChildCategories(
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

            return childCategories.Select(child => new GetAllCategoriesResponse
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
