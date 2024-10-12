using MediatR;

namespace TikiShop.Core.Services.BasketService.Commands;

public record DeleteBasketItemCommand(int UserId, int ProductId) : IRequest<ServiceResult>;