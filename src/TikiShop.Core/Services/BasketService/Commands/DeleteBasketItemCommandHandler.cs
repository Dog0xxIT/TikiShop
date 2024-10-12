using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TikiShop.Core.Services.CatalogService.Queries;
using TikiShop.Infrastructure;

namespace TikiShop.Core.Services.BasketService.Commands;

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
        var basket = await _context.Baskets
            .AsNoTracking()
            .SingleOrDefaultAsync(b => b.BuyerId == request.UserId);
        var basketId = basket?.Id;
        var basketItem = _context.BasketItems
            .AsNoTracking() // Notify
            .SingleOrDefault(i =>
                i.ProductId == request.ProductId &&
                i.BasketId == basketId);

        if (basketId is null || basketItem is null)
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