using TikiShop.Core.Services.CatalogService.Commands;
using TikiShop.Infrastructure;
using TikiShop.Infrastructure.Models;

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
            if (string.IsNullOrEmpty(request.Name))
            {
                return ServiceResult.Failed("Name Invalid");
            }

            var brand = new Brand
            {
                Name = request.Name,
                Slug = request.Name.ToLower().Replace(" ", "-")
            };

            try
            {
                await _context.Brands.AddAsync(brand);
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
