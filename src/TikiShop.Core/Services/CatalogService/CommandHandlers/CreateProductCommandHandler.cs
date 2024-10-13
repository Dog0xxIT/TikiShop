using TikiShop.Core.Services.BasketService.CommandHandlers;
using TikiShop.Core.Services.CatalogService.Commands;
using TikiShop.Infrastructure;
using TikiShop.Infrastructure.Models;

namespace TikiShop.Core.Services.CatalogService.CommandHandlers
{
    internal class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ServiceResult>
    {
        private readonly TikiShopDbContext _context;
        private readonly ILogger<CreateProductCommandHandler> _logger;

        public CreateProductCommandHandler(ILogger<CreateProductCommandHandler> logger, TikiShopDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<ServiceResult> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.Name))
            {
                return ServiceResult.Failed("Name Invalid");
            }

            if (request.Price <= 0)
            {
                return ServiceResult.Failed("Price Invalid");
            }

            if (request.Quantity < 0)
            {
                return ServiceResult.Failed("Price Invalid");
            }

            var isExistBrand = await _context.Brands
                .AsNoTracking()
                .AnyAsync(b => b.Id == request.BrandId);
            if (!isExistBrand)
            {
                return ServiceResult.Failed("Brand Not Existed");
            }

            var isExistCategory = await _context.Categories
                .AsNoTracking()
                .AnyAsync(b => b.Id == request.BrandId);
            if (!isExistCategory)
            {
                return ServiceResult.Failed("Category Not Existed");
            }

            var product = new Product
            {
                BrandId = request.BrandId,
                CategoryId = request.CategoryId,
                Description = request.Description,
                Name = request.Name,
                Price = request.Price,
                Quantity = request.Quantity,
                ShortDescription = request.ShortDescription,
                ThumbnailUrl = request.ThumbnailUrl,
                Sku = request.Sku
            };

            try
            {
                await _context.Products.AddAsync(product);
                await _context.SaveChangesAsync();
                return ServiceResult.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return ServiceResult.Failed();
            }
        }
    }
}
