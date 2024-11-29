using TikiShop.Core.Services.CatalogService.Commands;
using TikiShop.Infrastructure.Common;
using TikiShop.Model.DTO;

namespace TikiShop.Core.Services.OrderService.Commands;

public record CreateOrderCommand(
    int BuyerId,
    int AddressId,
    List<OrderItemDto> Items) : IRequest<ResultObject<int>>;

internal class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, ResultObject<int>>
{
    private readonly TikiShopDbContext _context;
    private readonly ILogService<CreateBrandCommandHandler> _logService;

    public CreateOrderCommandHandler(TikiShopDbContext context, ILogService<CreateBrandCommandHandler> logService)
    {
        _context = context;
        _logService = logService;
    }

    public async Task<ResultObject<int>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var isExistUser = await _context.Users
            .AsNoTracking()
            .AnyAsync(u => u.Id == request.BuyerId);
        if (!isExistUser)
        {
            _logService.LogWarning($"User with ID {request.BuyerId} does not exist.");
            return ResultObject<int>.Failed("Request Invalid");
        }

        var address = await _context.Addresses
            .AsNoTracking()
            .SingleOrDefaultAsync(a => a.Id == request.AddressId && a.UserId == request.BuyerId);
        if (address is null)
        {
            _logService.LogWarning($"Address with ID {request.AddressId} not found for user {request.BuyerId}.");
            return ResultObject<int>.Failed("Address Invalid");
        }

        if (string.IsNullOrEmpty(address.City) ||
            string.IsNullOrEmpty(address.Country) ||
            string.IsNullOrEmpty(address.PhoneNumber) ||
            string.IsNullOrEmpty(address.Receiver) ||
            string.IsNullOrEmpty(address.State) ||
            string.IsNullOrEmpty(address.Street))
        {
            _logService.LogWarning(
                $"Invalid address details for user {request.BuyerId}, address ID {request.AddressId}.");
            return ResultObject<int>.Failed("Address Invalid");
        }

        var order = new Order
        {
            AddressId = request.AddressId,
            BuyerId = request.BuyerId,
            OrderDate = DateTime.UtcNow,
            Description = "",
            PaymentDetailId = 0,
            OrderStatus = OrderStatus.Submitted,
            OrderItems = new List<OrderItem>(),
            Total = 0
        };

        foreach (var item in request.Items)
        {
            var product = await _context.ProductSkus
                .AsNoTracking()
                .Select(p => new
                {
                    p.Id,
                    QuantityStock = p.Quantity,
                    p.Price
                })
                .SingleOrDefaultAsync(p => p.Id == item.ProductSkuId);

            if (product is null || item.Quantity < 0 || item.Quantity > product.QuantityStock)
            {
                _logService.LogWarning(
                    $"Invalid item: {item.ProductSkuId}, Quantity: {item.Quantity}, Stock: {product?.QuantityStock}");
                return ResultObject<int>.Failed($"Item {item.ProductSkuId} Invalid");
            }

            order.OrderItems.Add(new OrderItem
            {
                ProductSkuId = item.ProductSkuId,
                Discount = 0,
                Quantity = item.Quantity
            });
            order.Total += item.Quantity * product.Price;
        }

        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return ResultObject<int>.Success();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logService.LogError($"Error while creating order: {ex.Message}");
            return ResultObject<int>.Failed("Error while creating order");
        }
    }
}