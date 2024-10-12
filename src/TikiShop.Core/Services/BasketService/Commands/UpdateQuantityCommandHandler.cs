using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TikiShop.Core.Services.CatalogService.Queries;
using TikiShop.Infrastructure;
using TikiShop.Infrastructure.Models;

namespace TikiShop.Core.Services.BasketService.Commands
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
                return ServiceResult.Failed("Invalid Request");
            }
            var basket = await _context.Baskets
                .AsNoTracking()
                .SingleOrDefaultAsync(b => b.BuyerId == request.UserId);
            var basketId = basket?.Id;
            var checkProduct = await _context.Products
                .AnyAsync(p => p.Id == request.ProductId);
            var checkProductVariant = await _context.ProductVariants
                .AnyAsync(p => p.Id == request.ProductId);
            var basketItem = _context.BasketItems
                .SingleOrDefault(i =>
                    i.ProductId == request.ProductId &&
                    i.BasketId == basketId);

            if (basketId is null || checkProduct == false || checkProductVariant == false || basketItem is null)
            {
                return ServiceResult.Failed("Invalid Request");
            }

            try
            {
                basketItem.Quantity = request.Quantity;
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
