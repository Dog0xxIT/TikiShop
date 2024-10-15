using TikiShop.Core.Services.CatalogService.Commands;
using TikiShop.Infrastructure;

namespace TikiShop.Core.Services.CatalogService.CommandHandlers
{
    internal class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ServiceResult>
    {
        private readonly TikiShopDbContext _context;
        private readonly ILogger<UpdateProductCommandHandler> _logger;

        public UpdateProductCommandHandler(ILogger<UpdateProductCommandHandler> logger, TikiShopDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<ServiceResult> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
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

            var product = await _context.Products.FindAsync(request.Id);
            if (product is null)
            {
                return ServiceResult.Failed("Product Not Existed");
            }

            product.BrandId = request.BrandId;
            product.CategoryId = request.CategoryId;
            product.Description = request.Description;
            product.Name = request.Name;
            //product.Price = request.Price;
            //product.Quantity = request.Quantity;
            product.Summary = request.ShortDescription;
            product.ThumbnailUrl = request.ThumbnailUrl;
            //product.Sku = request.Sku;

            try
            {
                _context.Products.Update(product);
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
