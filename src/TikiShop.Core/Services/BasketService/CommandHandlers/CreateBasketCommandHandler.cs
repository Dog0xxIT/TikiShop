using TikiShop.Core.Services.BasketService.Commands;
using TikiShop.Infrastructure;
using TikiShop.Infrastructure.Models;

namespace TikiShop.Core.Services.BasketService.CommandHandlers;

internal class CreateBasketCommandHandler : IRequestHandler<CreateBasketCommand, ServiceResult>
{
    private readonly TikiShopDbContext _context;
    private readonly ILogger<CreateBasketCommandHandler> _logger;

    public CreateBasketCommandHandler(ILogger<CreateBasketCommandHandler> logger, TikiShopDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<ServiceResult> Handle(CreateBasketCommand request, CancellationToken cancellationToken)
    {
        var isExistBasket = await _context.Baskets
            .AnyAsync(b => b.BuyerId == request.UserId);

        if (isExistBasket)
        {
            return ServiceResult.Failed("Basket Existed");
        }
        var basket = new Basket
        {
            BuyerId = request.UserId,
            Total = 0,
        };

        try
        {
            await _context.Baskets.AddAsync(basket);
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