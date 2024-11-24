using TikiShop.Model.DTO;

namespace TikiShop.Core.Services.CatalogService.Commands;

public record CreateBrandCommand(string Name) : IRequest<ResultObject<int>>;

internal class CreateBrandCommandHandler : IRequestHandler<CreateBrandCommand, ResultObject<int>>
{
    private readonly TikiShopDbContext _context;
    private readonly ILogService<CreateBrandCommandHandler> _logService;

    public CreateBrandCommandHandler(ILogService<CreateBrandCommandHandler> logService, TikiShopDbContext context)
    {
        _logService = logService;
        _context = context;
    }

    public async Task<ResultObject<int>> Handle(CreateBrandCommand request, CancellationToken cancellationToken)
    {
        // Log the incoming request with the brand name
        _logService.LogInformation($"Received CreateBrandCommand with Name: {request.Name}");

        // Validate the brand name
        if (string.IsNullOrEmpty(request.Name))
        {
            _logService.LogWarning("CreateBrandCommand failed due to invalid name.");
            return ResultObject<int>.Failed("Name Invalid");
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
            _logService.LogInformation($"Attempting to add new brand with Name: {brand.Name} and Slug: {brand.Slug}");

            await _context.Brands.AddAsync(brand);
            await _context.SaveChangesAsync();

            // Log success
            _logService.LogInformation($"Successfully created brand with Name: {brand.Name}");
            return ResultObject<int>.Success();
        }
        catch (Exception ex)
        {
            // Log detailed error information
            _logService.LogError(ex, $"An error occurred while creating brand with Name: {brand.Name}");
            return ResultObject<int>.Failed("An error occurred while creating the brand");
        }
    }
}