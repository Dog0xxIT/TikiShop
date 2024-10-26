using TikiShop.Core.Services.BasketService.CommandHandlers;
using TikiShop.Core.Services.CatalogService.Commands;
using TikiShop.Infrastructure;
using TikiShop.Infrastructure.Models;
using Microsoft.Extensions.Logging;

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
            // Log the received command with product details
            _logger.LogInformation($"Received CreateProductCommand with Name: {request.Name}, BrandId: {request.BrandId}, CategoryId: {request.CategoryId}, Price: {request.Price}, Quantity: {request.Quantity}");

            // Validate product name
            if (string.IsNullOrEmpty(request.Name))
            {
                _logger.LogWarning("CreateProductCommand failed due to invalid product name.");
                return ServiceResult.Failed("Name Invalid");
            }

            // Validate product price
            if (request.Price <= 0)
            {
                _logger.LogWarning($"CreateProductCommand failed due to invalid price: {request.Price}.");
                return ServiceResult.Failed("Price Invalid");
            }

            // Validate product quantity
            if (request.Quantity < 0)
            {
                _logger.LogWarning($"CreateProductCommand failed due to invalid quantity: {request.Quantity}.");
                return ServiceResult.Failed("Quantity Invalid");
            }

            // Check if the brand exists
            var isExistBrand = await _context.Brands
                .AsNoTracking()
                .AnyAsync(b => b.Id == request.BrandId);
            if (!isExistBrand)
            {
                _logger.LogWarning($"Brand with ID {request.BrandId} does not exist.");
                return ServiceResult.Failed("Brand Not Existed");
            }

            // Check if the category exists
            var isExistCategory = await _context.Categories
                .AsNoTracking()
                .AnyAsync(c => c.Id == request.CategoryId);
            if (!isExistCategory)
            {
                _logger.LogWarning($"Category with ID {request.CategoryId} does not exist.");
                return ServiceResult.Failed("Category Not Existed");
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
                _logger.LogInformation($"Attempting to add product with Name: {product.Name}.");

                await _context.Products.AddAsync(product);
                await _context.SaveChangesAsync();

                // Log success
                _logger.LogInformation($"Successfully created product with Name: {product.Name}, ID: {product.Id}");
                return ServiceResult.Success;
            }
            catch (Exception ex)
            {
                // Log the error details with exception information
                _logger.LogError(ex, $"An error occurred while creating product with Name: {product.Name}");
                return ServiceResult.Failed("An error occurred while creating the product");
            }
        }
    }
}
