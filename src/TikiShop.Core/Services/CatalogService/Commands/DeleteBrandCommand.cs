using TikiShop.Model.DTO;

namespace TikiShop.Core.Services.CatalogService.Commands;

public record DeleteBrandCommand(int Id) : IRequest<ResultObject<int>>;

internal class DeleteBrandHandler : IRequestHandler<DeleteBrandCommand, ResultObject<int>>
{
    private readonly TikiShopDbContext _context;
    private readonly ILogService<DeleteBrandHandler> _logService;

    public DeleteBrandHandler(ILogService<DeleteBrandHandler> logService, TikiShopDbContext context)
    {
        _logService = logService;
        _context = context;
    }

    public async Task<ResultObject<int>> Handle(DeleteBrandCommand request, CancellationToken cancellationToken)
    {
        // Log initial request information
        _logService.LogInformation($"Received DeleteBrandCommand with Brand ID: {request.Id}");

        // Attempt to retrieve the brand by ID
        var brand = await _context.Brands.FindAsync(request.Id);
        if (brand is null)
        {
            _logService.LogWarning($"DeleteBrandCommand failed: Brand with ID {request.Id} does not exist.");
            return ResultObject<int>.Failed("Brand Not Existed");
        }

        // Mark brand as deleted and set the last modified time
        brand.IsDeleted = true;
        brand.SetTimeLastModified();
        _logService.LogInformation($"Marking Brand ID {brand.Id} as deleted.");

        try
        {
            // Update the brand in the database
            _context.Brands.Update(brand);
            await _context.SaveChangesAsync();

            // Log success message
            _logService.LogInformation($"Successfully marked Brand ID {brand.Id} as deleted.");
            return ResultObject<int>.Success();
        }
        catch (Exception ex)
        {
            // Log the error with detailed exception information
            _logService.LogError(ex, $"An error occurred while attempting to delete Brand ID {brand.Id}");
            return ResultObject<int>.Failed("An error occurred while deleting the brand");
        }
    }
}