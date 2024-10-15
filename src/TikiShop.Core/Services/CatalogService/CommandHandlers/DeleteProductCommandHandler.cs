using TikiShop.Core.Services.CatalogService.Commands;
using TikiShop.Infrastructure;

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
            var product = await _context.Products
                .FindAsync(request.Id);
            if (product is null)
            {
                return ServiceResult.Failed("Product Not Existed");
            }

            var productSkus = await _context.ProductSkus
                .Where(pv => pv.ProductId == request.Id)
                .ToListAsync();
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                product.IsDeleted = true;
                product.SetTimeLastModified();
                _context.Products.Update(product);
                await _context.SaveChangesAsync();

                productSkus.ForEach(ps =>
                {
                    ps.IsDeleted = true;
                    ps.SetTimeLastModified();
                });
                await _context.BulkUpdateAsync(productSkus);
                await _context.BulkSaveChangesAsync();

                await transaction.CommitAsync();
                return ServiceResult.Success;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex.Message);
                return ServiceResult.Failed();
            }
        }
    }
}
