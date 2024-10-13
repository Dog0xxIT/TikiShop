namespace TikiShop.Core.Services.BasketService.Commands;

public record DeleteBasketItemCommand(int BaskItemId) : IRequest<ServiceResult>;