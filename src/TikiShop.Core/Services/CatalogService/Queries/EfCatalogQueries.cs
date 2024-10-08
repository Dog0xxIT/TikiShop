using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mapster;
using Microsoft.EntityFrameworkCore;
using TikiShop.Core.Dto;
using TikiShop.Core.Dto.Catalog;
using TikiShop.Infrastructure;
using TikiShop.Share.RequestModels;
using TikiShop.Share.RequestModels.Catalog;
using TikiShop.Share.ResponseModels;
using TikiShop.Share.ResponseModels.Catalog;
using TikiShop.Infrastructure.Models;

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

        public async Task<PaginationDto<GetListProductDto>> GetListProducts(GetListProductRequest req)
        {
            if (req.MinPrice > req.MaxPrice)
            {
                return new();
            }

            var brands = ParseIds(req.Brands);
            var categories = ParseIds(req.Categories);

            var queryable = _context.Products.AsQueryable();
            queryable = ApplyFilters(queryable, req, brands, categories);
            queryable = ApplySorting(queryable, req);

            var products = await queryable
                .Skip((req.Page - 1) * req.Limit)
                .Take(req.Limit)
                .ToListAsync();

            var productsDto = products.Select(product =>
                new GetListProductDto
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
                    //TotalBuyer = product.TotalBuyer,
                    Sku = product.Sku,
                    ShortDescription = product.ShortDescription,
                }).ToList();

            var totalProducts = await _context.Products.CountAsync();
            var totalPage = req.Limit != 0 ? (totalProducts / req.Limit) + 1 : 0;

            var response = new PaginationDto<GetListProductDto>
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

        public async Task<GetProductByIdDto> GetProductById(int id)
        {
           var product = await _context.Products
                 .Include(p => p.Brand)
                 .Include(p => p.Category)
                 .Include(p => p.ProductVariant)
                     .ThenInclude(vo => vo.OptionType1)
                     .SingleOrDefaultAsync(p => p.Id == id);

            if (product is null)
            {
                return new();
            }

            //var productOptions = product?.ProductVariant?.SelectMany(pv => pv.Options);
            //var groupByOptionType = productOptions?.GroupBy(po => po.OptionType);
            //var configOptions = groupByOptionType?.Select(g =>
            //    new GetProductByIdResponse.ConfigOption
            //    {
            //        OptionTypeId = g.Key.Id,
            //        OptionType = g.Key.Name,
            //        Values = g.Select(i => i.Value).ToList()
            //    }).ToList() ?? new();


            var productVariants = product?.ProductVariant?.Select(productVariant =>
                new GetProductByIdResponse.Variant
                {
                    ProductId = productVariant.ProductId,
                    ProductVariantId = productVariant.Id,
                    Sku = productVariant.Sku,
                    //AvailableStock = productVariant.AvailableStock,
                    Price = productVariant.Price,
                    //Option1 = productVariant?.Options?.ElementAtOrDefault(0)?.Value ?? "",
                    //Option2 = productVariant?.Options?.ElementAtOrDefault(1)?.Value ?? "",
                }).ToList() ?? new();

            var response = new GetProductByIdDto
            {
                Id = product!.Id,
                Name = product.Name,
                EmbedDescription = product.Description,
                Price = product.Price,
                ThumbnailUrl = product.ThumbnailUrl,
                //Discount = product.Discount,
                //ReviewCount = product.ReviewCount,
                //RatingAverage = product.RatingAverage,
                //TotalBuyer = product.TotalBuyer,
                Sku = product.Sku,
                ShortDescription = product.ShortDescription,
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
                //Variants = productVariants,
                //ConfigOptions = configOptions
            };

            return response;
        }

        public async Task<PaginationDto<GetListBrandsDto>> GetListBrands(PaginationRequest req)
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
            var brandsDto = brands.Adapt<List<GetListBrandsDto>>();

            var response = new PaginationDto<GetListBrandsDto>
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

        public async Task<List<GetAllCategoriesDto>> GetCategoriesHierarchy()
        {
            var categories = await _context.Categories
                .AsNoTracking()
                .ToListAsync();

            var categoriesDto = categories
                .Select(category => new GetAllCategoriesDto
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

        public async Task<PaginationDto<GetAllCategoriesDto>> GetCategories()
        {
            var categories = await _context.Categories.ToListAsync();

            var categoriesDto = categories.Select(category => new GetAllCategoriesDto
            {
                Id = category.Id,
                Name = category.Name,
                ParentId = category.ParentId,
                ThumbnailUrl = category.ThumbnailUrl,
                Childs = new(),
            }).ToList();

            var response = new PaginationDto<GetAllCategoriesDto>
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
            var sortByList = new Dictionary<string, string>
            {
                ["id"] = nameof(Product.Id),
                ["price"] = nameof(Product.Price),
                ["date_modified"] = nameof(Product.LastModified),
            };

            return sortByList[req.SortBy] switch
            {
                nameof(Product.Id) => req.SortDescending
                    ? queryable.OrderByDescending(product => product.Id)
                    : queryable.OrderBy(product => product.Id),
                nameof(Product.LastModified) => req.SortDescending
                    ? queryable.OrderByDescending(product => product.LastModified)
                    : queryable.OrderBy(product => product.LastModified),
                nameof(Product.Price) => req.SortDescending
                    ? queryable.OrderByDescending(product => product.Price)
                    : queryable.OrderBy(product => product.Price),
                _ => queryable
            };
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
                queryable = queryable.Where(product => product.Name.Contains(req.Keyword) || product.Description.Contains(req.Keyword));

            return queryable;
        }

        private List<GetAllCategoriesDto> GetChildCategories(
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

            return childCategories.Select(child => new GetAllCategoriesDto
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
