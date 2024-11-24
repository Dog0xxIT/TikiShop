using TikiShop.Model.DTO;

namespace TikiShop.Core.Services.BasketService.Commands;

public record UpdateQuantityCommand(int UserId, int ProductSkuId, int Quantity) : IRequest<ResultObject<int>>;

internal class UpdateQuantityCommandHandler : IRequestHandler<UpdateQuantityCommand, ResultObject<int>>
{
    private readonly TikiShopDbContext _context;
    private readonly ILogService<UpdateQuantityCommandHandler> _logService;

    public UpdateQuantityCommandHandler(TikiShopDbContext context, ILogService<UpdateQuantityCommandHandler> logService)
    {
        _context = context;
        _logService = logService;
    }

    public async Task<ResultObject<int>> Handle(UpdateQuantityCommand request, CancellationToken cancellationToken)
    {
        // Log the incoming request with details about the user, product, and requested quantity
        _logService.LogInformation(
            $"Received UpdateQuantityCommand for Email: {request.UserId}, ProductSkuId: {request.ProductSkuId}, Quantity: {request.Quantity}");

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
            _logService.LogWarning(
                $"Invalid quantity for ProductSkuId: {request.ProductSkuId}. Requested: {request.Quantity}, Available: {productSku?.Quantity ?? 0}");
            return ResultObject<int>.Failed("Invalid Quantity");
        }

        // Retrieve the basket for the user, including all items
        var basket = await _context.Baskets
            .Include(b => b.Items)
            .SingleOrDefaultAsync(b => b.BuyerId == request.UserId);

        // If basket not found, log warning and return failed result
        if (basket is null)
        {
            _logService.LogWarning($"Basket not found for Email: {request.UserId}");
            return ResultObject<int>.Failed("Invalid Request");
        }

        // Check if the basket contains the specified product SKU
        var basketItem = basket.Items.SingleOrDefault(bi => bi.ProductSkuId == request.ProductSkuId);
        if (basketItem is null)
        {
            // If item not found, log and add a new item to the basket
            _logService.LogInformation($"BasketItem not found, creating new item for ProductSkuId: {request.ProductSkuId}");
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
            _logService.LogInformation(
                $"Updating quantity for existing BasketItem with ProductSkuId: {request.ProductSkuId} to {request.Quantity}");
            basketItem.Quantity = request.Quantity;
        }

        // Recalculate the total basket price based on updated quantity and item price
        basket.Total += basketItem.Quantity * productSku.Price;

        try
        {
            // Attempt to save changes to the database and log success
            _logService.LogInformation($"Attempting to update basket for Email: {request.UserId}");

            _context.Update(basket);
            await _context.SaveChangesAsync();

            _logService.LogInformation($"Successfully updated basket for Email: {request.UserId}");
            return ResultObject<int>.Success();
        }
        catch (Exception ex)
        {
            // Log error details if any exception occurs during the update
            _logService.LogError(ex, $"An error occurred while updating basket for Email: {request.UserId}");
            return ResultObject<int>.Failed("An error occurred while updating the basket");
        }
    }
}