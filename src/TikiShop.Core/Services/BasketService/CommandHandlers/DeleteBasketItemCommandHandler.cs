using TikiShop.Core.Services.BasketService.Commands;
using TikiShop.Infrastructure;

namespace TikiShop.Core.Services.BasketService.CommandHandlers;

internal class DeleteBasketItemCommandHandler : IRequestHandler<DeleteBasketItemCommand, ServiceResult>
{
    private readonly TikiShopDbContext _context;
    private readonly ILogger<DeleteBasketItemCommandHandler> _logger;

    public DeleteBasketItemCommandHandler(ILogger<DeleteBasketItemCommandHandler> logger, TikiShopDbContext context)
    {
        _logger = logger;
        _context = context;
    }
    public async Task<ServiceResult> Handle(DeleteBasketItemCommand request, CancellationToken cancellationToken)
    {
        var basketItem = await _context.BasketItems
            .FindAsync(request.BaskItemId);

        if (basketItem is null)
        {
            return ServiceResult.Failed("Invalid Request");
        }

        try
        {
            _context.BasketItems.Remove(basketItem);
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