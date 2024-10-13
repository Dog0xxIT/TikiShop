using TikiShop.Core.Services.CatalogService.Commands;
using TikiShop.Infrastructure;

namespace TikiShop.Core.Services.CatalogService.CommandHandlers;

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
        var brand = await _context.Brands.FindAsync(request.Id);
        if (brand is null)
        {
            return ServiceResult.Failed("Brand Not Existed");
        }

        brand.Name = request.Name;
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