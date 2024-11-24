using TikiShop.Model.DTO;

namespace TikiShop.Core.Services.CatalogService.Commands;

public record UpdateProductCommand(
    int Id,
    string Sku,
    string Name,
    string? Description,
    string? ShortDescription,
    double Price,
    string? ThumbnailUrl,
    int Quantity,
    int CategoryId,
    int BrandId) : IRequest<ResultObject<int>>;

internal class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ResultObject<int>>
{
    private readonly TikiShopDbContext _context;
    private readonly ILogService<UpdateProductCommandHandler> _logService;


    public UpdateProductCommandHandler(ILogService<UpdateProductCommandHandler> logService, TikiShopDbContext context)
    {
        _logService = logService;
        _context = context;
    }

    public async Task<ResultObject<int>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        _logService.LogInformation($"Received UpdateProductCommand for Product ID: {request.Id}");

        // Validate request properties
        if (string.IsNullOrEmpty(request.Name))
        {
            _logService.LogWarning("Update failed: Invalid product name.");
            return ResultObject<int>.Failed("Name Invalid");
        }

        if (request.Price <= 0)
        {
            _logService.LogWarning("Update failed: Invalid product price.");
            return ResultObject<int>.Failed("Price Invalid");
        }

        if (request.Quantity < 0)
        {
            _logService.LogWarning("Update failed: Invalid product quantity.");
            return ResultObject<int>.Failed("Quantity Invalid");
        }

        // Check if the brand exists
        var isExistBrand = await _context.Brands
            .AsNoTracking()
            .AnyAsync(b => b.Id == request.BrandId);
        if (!isExistBrand)
        {
            _logService.LogWarning($"Update failed: Brand with ID {request.BrandId} does not exist.");
            return ResultObject<int>.Failed("Brand Not Existed");
        }

        // Check if the category exists
        var isExistCategory = await _context.Categories
            .AsNoTracking()
            .AnyAsync(c => c.Id == request.CategoryId); // Fixed to check CategoryId
        if (!isExistCategory)
        {
            _logService.LogWarning($"Update failed: Category with ID {request.CategoryId} does not exist.");
            return ResultObject<int>.Failed("Category Not Existed");
        }

        // Retrieve the product to update
        var product = await _context.Products.FindAsync(request.Id);
        if (product is null)
        {
            _logService.LogWarning($"Update failed: Product with ID {request.Id} does not exist.");
            return ResultObject<int>.Failed("Product Not Existed");
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
            _logService.LogInformation($"Successfully updated Product ID {product.Id}.");
            return ResultObject<int>.Success();
        }
        catch (Exception ex)
        {
            _logService.LogError(ex, $"An error occurred while updating Product ID {product.Id}: {ex.Message}");
            return ResultObject<int>.Failed("An error occurred while updating the product");
        }
    }
}