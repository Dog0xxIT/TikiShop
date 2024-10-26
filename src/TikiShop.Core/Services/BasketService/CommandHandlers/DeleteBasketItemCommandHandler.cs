using TikiShop.Core.Services.BasketService.Commands;
using TikiShop.Infrastructure;
using Microsoft.Extensions.Logging;

namespace TikiShop.Core.Services.BasketService.CommandHandlers
{
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
            // Log information about receiving requests
            _logger.LogInformation($"Received DeleteBasketItemCommand for BasketItemId: {request.BaskItemId}");

            var basketItem = await _context.BasketItems
                .FindAsync(request.BaskItemId);

            if (basketItem is null)
            {
                _logger.LogWarning($"BasketItem with ID {request.BaskItemId} not found");
                return ServiceResult.Failed("Invalid Request");
            }

            try
            {
                _logger.LogInformation($"Attempting to delete BasketItem with ID: {request.BaskItemId}");

                _context.BasketItems.Remove(basketItem);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Successfully deleted BasketItem with ID: {request.BaskItemId}");
                return ServiceResult.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting BasketItem with ID: {request.BaskItemId}");
                return ServiceResult.Failed("An error occurred while deleting the basket item");
            }
        }
    }
}
