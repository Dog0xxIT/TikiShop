using TikiShop.Core.Services.CatalogService.Commands;
using TikiShop.Infrastructure;
using Microsoft.Extensions.Logging;

namespace TikiShop.Core.Services.CatalogService.CommandHandlers
{
    internal class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, ServiceResult>
    {
        private readonly TikiShopDbContext _context;
        private readonly ILogger<DeleteProductCommandHandler> _logger;

        public DeleteProductCommandHandler(ILogger<DeleteProductCommandHandler> logger, TikiShopDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<ServiceResult> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Received DeleteProductCommand for Product ID: {request.Id}");

            var product = await _context.Products.FindAsync(request.Id);
            if (product is null)
            {
                _logger.LogWarning($"Delete failed: Product with ID {request.Id} does not exist.");
                return ServiceResult.Failed("Product Not Existed");
            }

            var productSkus = await _context.ProductSkus
                .Where(pv => pv.ProductId == request.Id)
                .ToListAsync();

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Mark the product as deleted
                product.IsDeleted = true;
                product.SetTimeLastModified();
                _context.Products.Update(product);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Product with ID {product.Id} marked as deleted.");

                // Mark all associated SKUs as deleted
                productSkus.ForEach(ps =>
                {
                    ps.IsDeleted = true;
                    ps.SetTimeLastModified();
                });
                await _context.BulkUpdateAsync(productSkus);
                await _context.BulkSaveChangesAsync();
                _logger.LogInformation($"Deleted {productSkus.Count} SKUs associated with Product ID {product.Id}.");

                await transaction.CommitAsync();
                return ServiceResult.Success;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, $"An error occurred while deleting Product ID {product.Id}: {ex.Message}");
                return ServiceResult.Failed("An error occurred while deleting the product.");
            }
        }
    }
}
