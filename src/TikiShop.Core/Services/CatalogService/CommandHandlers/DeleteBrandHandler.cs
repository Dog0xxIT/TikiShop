using TikiShop.Core.Services.CatalogService.Commands;
using TikiShop.Infrastructure;
using TikiShop.Infrastructure.Models;
using Microsoft.Extensions.Logging;

namespace TikiShop.Core.Services.CatalogService.CommandHandlers
{
    internal class DeleteBrandHandler : IRequestHandler<DeleteBrandCommand, ServiceResult>
    {
        private readonly TikiShopDbContext _context;
        private readonly ILogger<DeleteBrandHandler> _logger;

        public DeleteBrandHandler(ILogger<DeleteBrandHandler> logger, TikiShopDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<ServiceResult> Handle(DeleteBrandCommand request, CancellationToken cancellationToken)
        {
            // Log initial request information
            _logger.LogInformation($"Received DeleteBrandCommand with Brand ID: {request.Id}");

            // Attempt to retrieve the brand by ID
            var brand = await _context.Brands.FindAsync(request.Id);
            if (brand is null)
            {
                _logger.LogWarning($"DeleteBrandCommand failed: Brand with ID {request.Id} does not exist.");
                return ServiceResult.Failed("Brand Not Existed");
            }

            // Mark brand as deleted and set the last modified time
            brand.IsDeleted = true;
            brand.SetTimeLastModified();
            _logger.LogInformation($"Marking Brand ID {brand.Id} as deleted.");

            try
            {
                // Update the brand in the database
                _context.Brands.Update(brand);
                await _context.SaveChangesAsync();

                // Log success message
                _logger.LogInformation($"Successfully marked Brand ID {brand.Id} as deleted.");
                return ServiceResult.Success;
            }
            catch (Exception ex)
            {
                // Log the error with detailed exception information
                _logger.LogError(ex, $"An error occurred while attempting to delete Brand ID {brand.Id}");
                return ServiceResult.Failed("An error occurred while deleting the brand");
            }
        }
    }
}
