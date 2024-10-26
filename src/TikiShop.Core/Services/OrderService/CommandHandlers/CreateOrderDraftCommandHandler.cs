using System.Collections.ObjectModel;
using TikiShop.Core.Services.CatalogService.CommandHandlers;
using TikiShop.Core.Services.OrderService.Commands;
using TikiShop.Infrastructure;
using TikiShop.Infrastructure.Common;
using TikiShop.Infrastructure.Models;

namespace TikiShop.Core.Services.OrderService.CommandHandlers;

internal class CreateOrderDraftCommandHandler : IRequestHandler<CreateOrderDraftCommand, ServiceResult>
{
    private readonly TikiShopDbContext _context;
    private readonly ILogger<CreateOrderDraftCommandHandler> _logger;

    public CreateOrderDraftCommandHandler(TikiShopDbContext context, ILogger<CreateOrderDraftCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ServiceResult> Handle(CreateOrderDraftCommand request, CancellationToken cancellationToken)
    {
        var isExistUser = await _context.Users
                .AsNoTracking()
                .AnyAsync(u => u.Id == request.BuyerId);
        if (!isExistUser)
        {
            _logger.LogWarning($"User with ID {request.BuyerId} does not exist.");
            return ServiceResult.Failed("Request Invalid");
        }

        var address = await _context.Addresses
            .AsNoTracking()
            .SingleOrDefaultAsync(a => a.Id == request.AddressId && a.UserId == request.BuyerId);
        if (address is null)
        {
            _logger.LogWarning($"Address with ID {request.AddressId} not found for user {request.BuyerId}.");
            return ServiceResult.Failed("Address Invalid");
        }

        if (string.IsNullOrEmpty(address.City) ||
            string.IsNullOrEmpty(address.Country) ||
            string.IsNullOrEmpty(address.PhoneNumber) ||
            string.IsNullOrEmpty(address.Receiver) ||
            string.IsNullOrEmpty(address.State) ||
            string.IsNullOrEmpty(address.Street))
        {
            _logger.LogWarning($"Invalid address details for user {request.BuyerId}, address ID {request.AddressId}.");
            return ServiceResult.Failed("Address Invalid");
        }

        var order = new Order
        {
            AddressId = request.AddressId,
            BuyerId = request.BuyerId,
            OrderDate = DateTime.UtcNow,
            Description = "",
            PaymentDetailId = 0,
            OrderStatus = OrderStatus.Draft,
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
                _logger.LogWarning($"Invalid item: {item.ProductSkuId}, Quantity: {item.Quantity}, Stock: {product?.QuantityStock}");
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
            _logger.LogError($"Error while creating order draft: {ex.Message}");
            return ServiceResult.Failed();
        }
    }
}
