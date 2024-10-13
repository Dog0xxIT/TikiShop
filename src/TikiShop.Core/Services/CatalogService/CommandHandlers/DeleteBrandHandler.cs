using TikiShop.Core.Services.CatalogService.Commands;
using TikiShop.Infrastructure;
using TikiShop.Infrastructure.Models;

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
            var brand = await _context.Brands.FindAsync(request.Id);
            if (brand is null)
            {
                return ServiceResult.Failed("Brand Not Existed");
            }

            brand.IsDeleted = true;
            brand.SetTimeLastModified();
            try
            {
                _context.Brands.Update(brand);
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
