using TikiShop.Model.DTO;

namespace TikiShop.Core.Services.UserService.Commands;

public record DeleteAddressCommand(int Id) : IRequest<ResultObject<int>>;

internal class DeleteAddressCommandHandler : IRequestHandler<DeleteAddressCommand, ResultObject<int>>
{
    private readonly TikiShopDbContext _context;
    private readonly ILogService<DeleteAddressCommandHandler> _logService;

    public DeleteAddressCommandHandler(TikiShopDbContext context, ILogService<DeleteAddressCommandHandler> logService)
    {
        _context = context;
        _logService = logService;
    }

    public async Task<ResultObject<int>> Handle(DeleteAddressCommand request, CancellationToken cancellationToken)
    {
        _logService.LogInformation($"Starting to delete address with Id: {request.Id}");

        var address = await _context.Addresses.FindAsync(request.Id);

        if (address is null)
        {
            _logService.LogWarning($"Address not found for Id: {request.Id}");
            return ResultObject<int>.Failed("Invalid Request");
        }

        address.IsDeleted = true;
        address.SetTimeLastModified();

        try
        {
            _context.Addresses.Update(address);
            await _context.SaveChangesAsync();
            _logService.LogInformation($"Address with Id: {request.Id} deleted successfully.");
            return ResultObject<int>.Success();
        }
        catch (Exception ex)
        {
            _logService.LogError(ex, $"Error occurred while deleting address with Id: {request.Id}");
            return ResultObject<int>.Failed("Error occurred while deleting");
        }
    }
}