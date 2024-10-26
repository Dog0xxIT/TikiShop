using TikiShop.Core.Services.CatalogService.Commands;
using TikiShop.Infrastructure;
using TikiShop.Infrastructure.Models;
using Microsoft.Extensions.Logging;

namespace TikiShop.Core.Services.CatalogService.CommandHandlers
{
    internal class CreateBrandCommandHandler : IRequestHandler<CreateBrandCommand, ServiceResult>
    {
        private readonly TikiShopDbContext _context;
        private readonly ILogger<CreateBrandCommandHandler> _logger;

        public CreateBrandCommandHandler(ILogger<CreateBrandCommandHandler> logger, TikiShopDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<ServiceResult> Handle(CreateBrandCommand request, CancellationToken cancellationToken)
        {
            // Log the incoming request with the brand name
            _logger.LogInformation($"Received CreateBrandCommand with Name: {request.Name}");

            // Validate the brand name
            if (string.IsNullOrEmpty(request.Name))
            {
                _logger.LogWarning("CreateBrandCommand failed due to invalid name.");
                return ServiceResult.Failed("Name Invalid");
            }

            // Create a new brand entity
            var brand = new Brand
            {
                Name = request.Name,
                Slug = request.Name.ToLower().Replace(" ", "-")
            };

            try
            {
                // Log attempt to add new brand
                _logger.LogInformation($"Attempting to add new brand with Name: {brand.Name} and Slug: {brand.Slug}");
                
                await _context.Brands.AddAsync(brand);
                await _context.SaveChangesAsync();

                // Log success
                _logger.LogInformation($"Successfully created brand with Name: {brand.Name}");
                return ServiceResult.Success;
            }
            catch (Exception ex)
            {
                // Log detailed error information
                _logger.LogError(ex, $"An error occurred while creating brand with Name: {brand.Name}");
                return ServiceResult.Failed("An error occurred while creating the brand");
            }
        }
    }
}
