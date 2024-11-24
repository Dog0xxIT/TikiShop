using TikiShop.Model.DTO;

namespace TikiShop.Core.Services.CatalogService.Commands;

public record CreateProductCommand(
    string Sku,
    string Name,
    string? Description,
    string? Summary,
    double Price,
    string? ThumbnailUrl,
    int Quantity,
    int CategoryId,
    int BrandId) : IRequest<ResultObject<int>>;

internal class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ResultObject<int>>
{
    private readonly TikiShopDbContext _context;
    private readonly ILogService<CreateProductCommandHandler> _logService;

    public CreateProductCommandHandler(ILogService<CreateProductCommandHandler> logService, TikiShopDbContext context)
    {
        _logService = logService;
        _context = context;
    }

    public async Task<ResultObject<int>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        // Log the received command with product details
        _logService.LogInformation(
            $"Received CreateProductCommand with Name: {request.Name}, BrandId: {request.BrandId}, CategoryId: {request.CategoryId}, Price: {request.Price}, Quantity: {request.Quantity}");

        // Validate product name
        if (string.IsNullOrEmpty(request.Name))
        {
            _logService.LogWarning("CreateProductCommand failed due to invalid product name.");
            return ResultObject<int>.Failed("Name Invalid");
        }

        // Validate product price
        if (request.Price <= 0)
        {
            _logService.LogWarning($"CreateProductCommand failed due to invalid price: {request.Price}.");
            return ResultObject<int>.Failed("Price Invalid");
        }

        // Validate product quantity
        if (request.Quantity < 0)
        {
            _logService.LogWarning($"CreateProductCommand failed due to invalid quantity: {request.Quantity}.");
            return ResultObject<int>.Failed("Quantity Invalid");
        }

        // Check if the brand exists
        var isExistBrand = await _context.Brands
            .AsNoTracking()
            .AnyAsync(b => b.Id == request.BrandId);
        if (!isExistBrand)
        {
            _logService.LogWarning($"Brand with ID {request.BrandId} does not exist.");
            return ResultObject<int>.Failed("Brand Not Existed");
        }

        // Check if the category exists
        var isExistCategory = await _context.Categories
            .AsNoTracking()
            .AnyAsync(c => c.Id == request.CategoryId);
        if (!isExistCategory)
        {
            _logService.LogWarning($"Category with ID {request.CategoryId} does not exist.");
            return ResultObject<int>.Failed("Category Not Existed");
        }

        // Initialize the new product entity
        var product = new Product
        {
            BrandId = request.BrandId,
            CategoryId = request.CategoryId,
            Description = request.Description ?? "",
            Name = request.Name,
            Summary = request.Summary ?? "",
            ThumbnailUrl = request.ThumbnailUrl ?? ""
        };

        try
        {
            // Log attempt to add the new product
            _logService.LogInformation($"Attempting to add product with Name: {product.Name}.");

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            // Log success
            _logService.LogInformation($"Successfully created product with Name: {product.Name}, ID: {product.Id}");
            return ResultObject<int>.Success();
        }
        catch (Exception ex)
        {
            // Log the error details with exception information
            _logService.LogError(ex, $"An error occurred while creating product with Name: {product.Name}");
            return ResultObject<int>.Failed("An error occurred while creating the product");
        }
    }
}