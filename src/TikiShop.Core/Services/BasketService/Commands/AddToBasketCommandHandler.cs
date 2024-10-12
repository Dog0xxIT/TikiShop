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

namespace TikiShop.Core.Services.BasketService.Commands
{
    internal class AddToBasketCommandHandler : IRequestHandler<AddToBasketCommand, ServiceResult>
    {
        private readonly TikiShopDbContext _context;
        private readonly ILogger<AddToBasketCommandHandler> _logger;

        public AddToBasketCommandHandler(ILogger<AddToBasketCommandHandler> logger, TikiShopDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<ServiceResult> Handle(AddToBasketCommand request, CancellationToken cancellationToken)
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

            var isExistBasketItem = basket!.Items.Any(i => i.ProductId == request.ProductId);
            if (isExistBasketItem)
            {
                return ServiceResult.Failed("Invalid Request");
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                basket.Items.Add(new BasketItem
                {
                    ProductId = request.ProductId,
                    BasketId = basket.Id,
                    Quantity = 1
                });

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return ServiceResult.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                await transaction.RollbackAsync();
                return ServiceResult.Failed("Error");
            }
        }
    }
}
