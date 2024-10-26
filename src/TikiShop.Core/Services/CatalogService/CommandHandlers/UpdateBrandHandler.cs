using TikiShop.Core.Services.CatalogService.Commands;
using TikiShop.Infrastructure;
using Microsoft.Extensions.Logging;

namespace TikiShop.Core.Services.CatalogService.CommandHandlers
{
    public class UpdateBrandCommandHandler : IRequestHandler<UpdateBrandCommand, ServiceResult>
    {
        private readonly TikiShopDbContext _context;
        private readonly ILogger<UpdateBrandCommandHandler> _logger;

        public UpdateBrandCommandHandler(ILogger<UpdateBrandCommandHandler> logger, TikiShopDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<ServiceResult> Handle(UpdateBrandCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Received UpdateBrandCommand for Brand ID: {request.Id}");

            var brand = await _context.Brands.FindAsync(request.Id);
            if (brand is null)
            {
                _logger.LogWarning($"Update failed: Brand with ID {request.Id} does not exist.");
                return ServiceResult.Failed("Brand Not Existed");
            }

            brand.Name = request.Name;
            brand.SetTimeLastModified();
            
            try
            {
                _context.Brands.Update(brand);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Successfully updated Brand ID {brand.Id}.");
                return ServiceResult.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while updating Brand ID {brand.Id}: {ex.Message}");
                return ServiceResult.Failed("An error occurred while updating the brand");
            }
        }
    }
}