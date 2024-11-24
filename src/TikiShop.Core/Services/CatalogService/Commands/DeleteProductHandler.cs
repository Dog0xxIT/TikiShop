using TikiShop.Model.DTO;

namespace TikiShop.Core.Services.CatalogService.Commands;

public record DeleteProductCommand(int Id) : IRequest<ResultObject<int>>;

internal class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, ResultObject<int>>
{
    private readonly TikiShopDbContext _context;
    private readonly ILogService<DeleteProductCommandHandler> _logService;

    public DeleteProductCommandHandler(ILogService<DeleteProductCommandHandler> logService, TikiShopDbContext context)
    {
        _logService = logService;
        _context = context;
    }

    public async Task<ResultObject<int>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        _logService.LogInformation($"Received DeleteProductCommand for Product ID: {request.Id}");

        var product = await _context.Products.FindAsync(request.Id);
        if (product is null)
        {
            _logService.LogWarning($"Delete failed: Product with ID {request.Id} does not exist.");
            return ResultObject<int>.Failed("Product Not Existed");
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
            _logService.LogInformation($"Product with ID {product.Id} marked as deleted.");

            // Mark all associated SKUs as deleted
            productSkus.ForEach(ps =>
            {
                ps.IsDeleted = true;
                ps.SetTimeLastModified();
            });
            await _context.BulkUpdateAsync(productSkus);
            await _context.BulkSaveChangesAsync();
            _logService.LogInformation($"Deleted {productSkus.Count} SKUs associated with Product ID {product.Id}.");

            await transaction.CommitAsync();
            return ResultObject<int>.Success();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logService.LogError(ex, $"An error occurred while deleting Product ID {product.Id}: {ex.Message}");
            return ResultObject<int>.Failed("An error occurred while deleting the product.");
        }
    }
}