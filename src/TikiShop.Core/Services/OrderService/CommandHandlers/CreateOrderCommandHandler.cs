using System.Collections.ObjectModel;
using TikiShop.Core.Services.CatalogService.CommandHandlers;
using TikiShop.Core.Services.OrderService.Commands;
using TikiShop.Infrastructure;
using TikiShop.Infrastructure.Common;
using TikiShop.Infrastructure.Models;
using static TikiShop.Shared.ResponseModels.Basket.GetBasketByCustomerIdResponse;

namespace TikiShop.Core.Services.OrderService.CommandHandlers
{
    internal class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, ServiceResult>
    {
        private readonly TikiShopDbContext _context;
        private readonly ILogger<CreateBrandCommandHandler> _logger;

        public CreateOrderCommandHandler(TikiShopDbContext context, ILogger<CreateBrandCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ServiceResult> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var isExistUser = await _context.Users
                .AsNoTracking()
                .AnyAsync(u => u.Id == request.BuyerId);
            if (!isExistUser)
            {
                return ServiceResult.Failed("Request Invalid");
            }

            var address = await _context.Addresses
                .AsNoTracking()
                .SingleOrDefaultAsync(a => a.Id == request.AddressId &&
                                           a.UserId == request.BuyerId);
            if (address is null)
            {
                return ServiceResult.Failed("Address Invalid");
            }

            if (string.IsNullOrEmpty(address.City) ||
                string.IsNullOrEmpty(address.Country) ||
                string.IsNullOrEmpty(address.PhoneNumber) ||
                string.IsNullOrEmpty(address.Receiver) ||
                string.IsNullOrEmpty(address.State) ||
                string.IsNullOrEmpty(address.Street))
            {
                return ServiceResult.Failed("Address Invalid");
            }

            var order = new Order
            {
                AddressId = request.AddressId,
                BuyerId = request.BuyerId,
                OrderDate = DateTime.UtcNow,
                Description = "",
                PaymentDetailId = 0,
                OrderStatus = OrderStatus.Submitted,
                OrderItems = new(),
                Total = 0
            };
            foreach (var item in request.Items)
            {
                var product = await _context.ProductSkus
                    .AsNoTracking()
                    .Select(p => new
                    {
                        Id = p.Id,
                        QuantityStock = p.Quantity,
                        Price = p.Price
                    })
                    .SingleOrDefaultAsync(p => p.Id == item.ProductSkuId);

                if (product is null || item.Quantity < 0 || item.Quantity > product.QuantityStock)
                {
                    return ServiceResult.Failed($"Item {item.ProductSkuId} Invalid");
                }

                order.OrderItems.Add(new OrderItem
                {
                    ProductSkuId = item.ProductSkuId,
                    Discount = 0,
                    Quantity = item.Quantity,
                });
                order.Total += item.Quantity * product.Price;
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _context.Orders.AddAsync(order);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return ServiceResult.Success;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex.Message);
                return ServiceResult.Failed();
            }
        }
    }
}
