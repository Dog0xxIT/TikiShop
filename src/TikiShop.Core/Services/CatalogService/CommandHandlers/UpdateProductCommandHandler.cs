using TikiShop.Core.Services.CatalogService.Commands;
using TikiShop.Infrastructure;
using Microsoft.Extensions.Logging;

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
            _logger.LogInformation($"Received UpdateProductCommand for Product ID: {request.Id}");

            // Validate request properties
            if (string.IsNullOrEmpty(request.Name))
            {
                _logger.LogWarning("Update failed: Invalid product name.");
                return ServiceResult.Failed("Name Invalid");
            }

            if (request.Price <= 0)
            {
                _logger.LogWarning("Update failed: Invalid product price.");
                return ServiceResult.Failed("Price Invalid");
            }

            if (request.Quantity < 0)
            {
                _logger.LogWarning("Update failed: Invalid product quantity.");
                return ServiceResult.Failed("Quantity Invalid");
            }

            // Check if the brand exists
            var isExistBrand = await _context.Brands
                .AsNoTracking()
                .AnyAsync(b => b.Id == request.BrandId);
            if (!isExistBrand)
            {
                _logger.LogWarning($"Update failed: Brand with ID {request.BrandId} does not exist.");
                return ServiceResult.Failed("Brand Not Existed");
            }

            // Check if the category exists
            var isExistCategory = await _context.Categories
                .AsNoTracking()
                .AnyAsync(c => c.Id == request.CategoryId);  // Fixed to check CategoryId
            if (!isExistCategory)
            {
                _logger.LogWarning($"Update failed: Category with ID {request.CategoryId} does not exist.");
                return ServiceResult.Failed("Category Not Existed");
            }

            // Retrieve the product to update
            var product = await _context.Products.FindAsync(request.Id);
            if (product is null)
            {
                _logger.LogWarning($"Update failed: Product with ID {request.Id} does not exist.");
                return ServiceResult.Failed("Product Not Existed");
            }

            // Update product properties
            product.BrandId = request.BrandId;
            product.CategoryId = request.CategoryId;
            product.Description = request.Description;
            product.Name = request.Name;
            // Uncomment if needed
            // product.Price = request.Price;
            // product.Quantity = request.Quantity;
            product.Summary = request.ShortDescription;
            product.ThumbnailUrl = request.ThumbnailUrl;
            // Uncomment if needed
            // product.Sku = request.Sku;

            try
            {
                _context.Products.Update(product);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Successfully updated Product ID {product.Id}.");
                return ServiceResult.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while updating Product ID {product.Id}: {ex.Message}");
                return ServiceResult.Failed("An error occurred while updating the product");
            }
        }
    }
}
