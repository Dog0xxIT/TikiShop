using TikiShop.Core.Services.BasketService.Commands;
using TikiShop.Infrastructure;
using TikiShop.Infrastructure.Models;

namespace TikiShop.Core.Services.BasketService.CommandHandlers
{
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
            // Log information about receiving requests
            _logger.LogInformation($"Received CreateBasketCommand for UserId: {request.UserId}");

            var isExistBasket = await _context.Baskets
                .AnyAsync(b => b.BuyerId == request.UserId);

            if (isExistBasket)
            {
                _logger.LogWarning($"Basket already exists for UserId: {request.UserId}");
                return ServiceResult.Failed("Basket Existed");
            }

            var basket = new Basket
            {
                BuyerId = request.UserId,
                Total = 0,
            };

            try
            {
                _logger.LogInformation($"Attempting to create a new basket for UserId: {request.UserId}");

                await _context.Baskets.AddAsync(basket);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Successfully created a new basket for UserId: {request.UserId}");
                return ServiceResult.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while creating a basket for UserId: {request.UserId}");
                return ServiceResult.Failed("An error occurred while creating the basket");
            }
        }
    }
}
