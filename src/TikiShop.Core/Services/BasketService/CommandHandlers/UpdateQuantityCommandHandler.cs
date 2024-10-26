using TikiShop.Core.Services.BasketService.Commands;
using TikiShop.Infrastructure;
using TikiShop.Infrastructure.Models;
using Microsoft.Extensions.Logging;

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
            // Log the incoming request with details about the user, product, and requested quantity
            _logger.LogInformation($"Received UpdateQuantityCommand for UserId: {request.UserId}, ProductSkuId: {request.ProductSkuId}, Quantity: {request.Quantity}");

            // Retrieve product SKU details, including price and available quantity
            var productSku = await _context.ProductSkus
                .AsNoTracking()
                .Select(p => new
                {
                    p.Id,
                    p.Price,
                    p.Quantity
                })
                .SingleOrDefaultAsync(p => p.Id == request.ProductSkuId);

            // Validate requested quantity; log warning if invalid
            if (request.Quantity <= 0 || productSku is null || request.Quantity > productSku.Quantity)
            {
                _logger.LogWarning($"Invalid quantity for ProductSkuId: {request.ProductSkuId}. Requested: {request.Quantity}, Available: {productSku?.Quantity ?? 0}");
                return ServiceResult.Failed("Invalid Quantity");
            }

            // Retrieve the basket for the user, including all items
            var basket = await _context.Baskets
                .Include(b => b.Items)
                .SingleOrDefaultAsync(b => b.BuyerId == request.UserId);
            
            // If basket not found, log warning and return failed result
            if (basket is null)
            {
                _logger.LogWarning($"Basket not found for UserId: {request.UserId}");
                return ServiceResult.Failed("Invalid Request");
            }

            // Check if the basket contains the specified product SKU
            var basketItem = basket.Items.SingleOrDefault(bi => bi.ProductSkuId == request.ProductSkuId);
            if (basketItem is null)
            {
                // If item not found, log and add a new item to the basket
                _logger.LogInformation($"BasketItem not found, creating new item for ProductSkuId: {request.ProductSkuId}");
                basketItem = new BasketItem
                {
                    BasketId = basket.Id,
                    ProductSkuId = request.ProductSkuId,
                    Quantity = request.Quantity
                };
                basket.Items.Add(basketItem);
            }
            else
            {
                // If item exists, log and update its quantity
                _logger.LogInformation($"Updating quantity for existing BasketItem with ProductSkuId: {request.ProductSkuId} to {request.Quantity}");
                basketItem.Quantity = request.Quantity;
            }

            // Recalculate the total basket price based on updated quantity and item price
            basket.Total += basketItem.Quantity * productSku.Price;

            try
            {
                // Attempt to save changes to the database and log success
                _logger.LogInformation($"Attempting to update basket for UserId: {request.UserId}");
                
                _context.Update(basket);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Successfully updated basket for UserId: {request.UserId}");
                return ServiceResult.Success;
            }
            catch (Exception ex)
            {
                // Log error details if any exception occurs during the update
                _logger.LogError(ex, $"An error occurred while updating basket for UserId: {request.UserId}");
                return ServiceResult.Failed("An error occurred while updating the basket");
            }
        }
    }
}
