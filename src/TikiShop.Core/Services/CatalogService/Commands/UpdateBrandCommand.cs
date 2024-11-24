using TikiShop.Model.DTO;

namespace TikiShop.Core.Services.CatalogService.Commands;

public record UpdateBrandCommand(int Id, string Name) : IRequest<ResultObject<int>>;

internal class UpdateBrandCommandHandler : IRequestHandler<UpdateBrandCommand, ResultObject<int>>
{
    private readonly TikiShopDbContext _context;
    private readonly ILogService<UpdateBrandCommandHandler> _logService;

    public UpdateBrandCommandHandler(ILogService<UpdateBrandCommandHandler> logService, TikiShopDbContext context)
    {
        _logService = logService;
        _context = context;
    }

    public async Task<ResultObject<int>> Handle(UpdateBrandCommand request, CancellationToken cancellationToken)
    {
        _logService.LogInformation($"Received UpdateBrandCommand for Brand ID: {request.Id}");

        var brand = await _context.Brands.FindAsync(request.Id);
        if (brand is null)
        {
            _logService.LogWarning($"Update failed: Brand with ID {request.Id} does not exist.");
            return ResultObject<int>.Failed("Brand Not Existed");
        }

        brand.Name = request.Name;
        brand.SetTimeLastModified();

        try
        {
            _context.Brands.Update(brand);
            await _context.SaveChangesAsync();
            _logService.LogInformation($"Successfully updated Brand ID {brand.Id}.");
            return ResultObject<int>.Success();
        }
        catch (Exception ex)
        {
            _logService.LogError(ex, $"An error occurred while updating Brand ID {brand.Id}: {ex.Message}");
            return ResultObject<int>.Failed("An error occurred while updating the brand");
        }
    }
}