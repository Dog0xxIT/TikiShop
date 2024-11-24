using TikiShop.Core.Services.AuthService.Commands;
using TikiShop.Model.DTO;

namespace TikiShop.Core.Services.BasketService.Commands;

public record CreateBasketCommand(int UserId) : IRequest<ResultObject<int>>;

internal class CreateBasketCommandHandler : IRequestHandler<CreateBasketCommand, ResultObject<int>>
{
    private readonly TikiShopDbContext _context;
    private readonly ILogService<CreateBasketCommandHandler> _logService;

    public CreateBasketCommandHandler(ILogService<CreateBasketCommandHandler> logService, TikiShopDbContext context)
    {
        _logService = logService;
        _context = context;
    }

    public async Task<ResultObject<int>> Handle(CreateBasketCommand request, CancellationToken cancellationToken)
    {
        // Log information about receiving requests
        _logService.LogInformation($"Received CreateBasketCommand for Email: {request.UserId}");

        var isExistBasket = await _context.Baskets
            .AnyAsync(b => b.BuyerId == request.UserId);

        if (isExistBasket)
        {
            _logService.LogWarning($"Basket already exists for Email: {request.UserId}");
            return ResultObject<int>.Failed("Basket Existed");
        }

        var basket = new Basket
        {
            BuyerId = request.UserId,
            Total = 0
        };

        try
        {
            _logService.LogInformation($"Attempting to create a new basket for Email: {request.UserId}");

            await _context.Baskets.AddAsync(basket);
            await _context.SaveChangesAsync();

            _logService.LogInformation($"Successfully created a new basket for Email: {request.UserId}");
            return ResultObject<int>.Success();
        }
        catch (Exception ex)
        {
            _logService.LogError(ex, $"An error occurred while creating a basket for Email: {request.UserId}");
            return ResultObject<int>.Failed("An error occurred while creating the basket");
        }
    }
}