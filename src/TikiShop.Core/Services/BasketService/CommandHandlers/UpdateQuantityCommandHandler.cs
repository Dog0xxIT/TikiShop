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
            var basket = await _context.Baskets
                .AsNoTracking()
                .SingleOrDefaultAsync(b => b.BuyerId == request.UserId);
            if (basket is null)
            {
                return ServiceResult.Failed("Invalid Request");
            }

            var product = await _context.Products
                .AsNoTracking()
                .Select(p => new { Id = p.Id, QuantityStock = p.Quantity })
                .SingleOrDefaultAsync(p => p.Id == request.ProductId);

            if (product is null)
            {
                return ServiceResult.Failed("Invalid ProductId");
            }
            if (request.Quantity <= 0 || request.Quantity > product.QuantityStock)
            {
                return ServiceResult.Failed("Invalid Quantity");
            }

            var isAnyProductVariant = await _context.ProductVariants
                .AnyAsync(pv => pv.ProductId == request.ProductId);
            if (isAnyProductVariant)
            {
                var isExistProductVariant = await _context.ProductVariants
                        .AnyAsync(pv => pv.Id == request.ProductVariantId);
                if (!isExistProductVariant)
                {
                    return ServiceResult.Failed("Please Select Product Variant");
                }
            }

            var basketItem = await _context.BasketItems
                .SingleOrDefaultAsync(bi => bi.BasketId == basket.Id &&
                                            bi.ProductId == request.ProductId &&
                                            bi.ProductVariantId == request.ProductVariantId);


            try
            {
                if (basketItem is null)
                {
                    basketItem = new BasketItem
                    {
                        BasketId = basket.Id,
                        ProductId = request.ProductId,
                        ProductVariantId = request.ProductVariantId,
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
