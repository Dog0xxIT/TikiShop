using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TikiShop.Core.Services.CatalogService.Queries;
using TikiShop.Infrastructure;
using TikiShop.Infrastructure.Models;

namespace TikiShop.Core.Services.BasketService.CommandHandlers
{
    internal class UpdateQuantityCommandHandler : IRequestHandler<UpdateQuantityCommand, ServiceResult>
    {
        private readonly TikiShopDbContext _context;
        private readonly ILogger<EfCatalogQueries> _logger;

        public UpdateQuantityCommandHandler(TikiShopDbContext context, ILogger<EfCatalogQueries> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ServiceResult> Handle(UpdateQuantityCommand request, CancellationToken cancellationToken)
        {
            var basket = await _context.Baskets
                .Include(b => b.Items)
                .SingleOrDefaultAsync(b => b.BuyerId == request.UserId);
            var checkProduct = await _context.Products
                .AnyAsync(p => p.Id == request.ProductId);
            var checkProductVariant = await _context.ProductVariants
                .AnyAsync(p => p.Id == request.ProductId);

            if (basket is null && checkProduct == false && checkProductVariant == false)
            {
                return ServiceResult.Failed("Invalid Request");
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var basketItem = basket!.Items.SingleOrDefault(i => i.ProductId == request.ProductId);
                if (basketItem is null)
                {
                    if (request.Quantity <= 0)
                    {
                        return ServiceResult.Success;
                    }

                    basket.Items.Add(new BasketItem
                    {
                        ProductId = request.ProductId,
                        BasketId = basket.Id,
                        Quantity = request.Quantity
                    });
                }
                else
                {
                    if (request.Quantity <= 0)
                    {
                        basket.Items.Remove(basketItem);
                    }
                    else
                    {
                        basketItem.Quantity = request.Quantity;
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return ServiceResult.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                await transaction.RollbackAsync();
                return ServiceResult.Failed();
            }
        }
    }
}
