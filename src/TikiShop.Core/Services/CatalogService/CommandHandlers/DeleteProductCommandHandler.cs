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
            var product = await _context.Products.FindAsync(request.Id);
            if (product is null)
            {
                return ServiceResult.Failed("Product Not Existed");
            }

            var productVariants = await _context.ProductVariants
                .Where(pv => pv.ProductId == request.Id)
                .ToListAsync();

            product.IsDeleted = true;
            product.SetTimeLastModified();
            productVariants.ForEach(pv =>
            {
                pv.IsDeleted = true;
                pv.SetTimeLastModified();
            });

            try
            {
                _context.Products.Update(product);
                _context.ProductVariants.UpdateRange(productVariants);
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
