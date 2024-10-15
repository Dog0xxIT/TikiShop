using TikiShop.Core.Services.BasketService.Commands;
using TikiShop.Infrastructure;
using TikiShop.Infrastructure.Models;

namespace TikiShop.Core.Services.BasketService.CommandHandlers
{
    internal class UpdateQuantityCommandHandler : IRequestHandler<UpdateQuantityCommand, ServiceResult>
    {
        private readonly TikiShopDbContext _context;
        private readonly ILogger<UpdateQuantityCommandHandler> _logger;

        public UpdateQuantityCommandHandler(TikiShopDbContext context, ILogger<UpdateQuantityCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ServiceResult> Handle(UpdateQuantityCommand request, CancellationToken cancellationToken)
        {
            if (request.Quantity <= 0)
            {
                return ServiceResult.Failed("Invalid Quantity");
            }

            var basket = await _context.Baskets
                .AsNoTracking()
                .SingleOrDefaultAsync(b => b.BuyerId == request.UserId);
            if (basket is null)
            {
                return ServiceResult.Failed("Invalid Request");
            }

            var product = await _context.ProductSkus
                .FindAsync(request.ProductSkuId);

            if (product is null || request.Quantity > product.Quantity)
            {
                return ServiceResult.Failed("Invalid Quantity");
            }

            var basketItem = await _context.BasketItems
                .SingleOrDefaultAsync(bi => bi.BasketId == basket.Id &&
                                            bi.ProductSkuId == request.ProductSkuId);

            try
            {
                if (basketItem is null)
                {
                    basketItem = new BasketItem
                    {
                        BasketId = basket.Id,
                        ProductSkuId = request.ProductSkuId,
                        Quantity = request.Quantity
                    };
                    await _context.BasketItems.AddAsync(basketItem);
                }
                else
                {
                    basketItem.Quantity = request.Quantity;
                    _context.BasketItems.Update(basketItem);
                }
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
