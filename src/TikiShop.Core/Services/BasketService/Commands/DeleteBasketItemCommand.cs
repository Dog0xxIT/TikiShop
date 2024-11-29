using TikiShop.Model.DTO;

namespace TikiShop.Core.Services.BasketService.Commands;

public record DeleteBasketItemCommand(int BaskItemId) : IRequest<ResultObject<int>>;

internal class DeleteBasketItemCommandHandler : IRequestHandler<DeleteBasketItemCommand, ResultObject<int>>
{
    private readonly TikiShopDbContext _context;
    private readonly ILogService<DeleteBasketItemCommandHandler> _logService;

    public DeleteBasketItemCommandHandler(ILogService<DeleteBasketItemCommandHandler> logService,
        TikiShopDbContext context)
    {
        _logService = logService;
        _context = context;
    }

    public async Task<ResultObject<int>> Handle(DeleteBasketItemCommand request, CancellationToken cancellationToken)
    {
        // Log information about receiving requests
        _logService.LogInformation($"Received DeleteBasketItemCommand for BasketItemId: {request.BaskItemId}");

        var basketItem = await _context.BasketItems
            .FindAsync(request.BaskItemId);

        if (basketItem is null)
        {
            _logService.LogWarning($"BasketItem with ID {request.BaskItemId} not found");
            return ResultObject<int>.Failed("Invalid Request");
        }

        try
        {
            _logService.LogInformation($"Attempting to delete BasketItem with ID: {request.BaskItemId}");

            _context.BasketItems.Remove(basketItem);
            await _context.SaveChangesAsync();

            _logService.LogInformation($"Successfully deleted BasketItem with ID: {request.BaskItemId}");
            return ResultObject<int>.Success();
        }
        catch (Exception ex)
        {
            _logService.LogError(ex, $"An error occurred while deleting BasketItem with ID: {request.BaskItemId}");
            return ResultObject<int>.Failed("An error occurred while deleting the basket item");
        }
    }
}