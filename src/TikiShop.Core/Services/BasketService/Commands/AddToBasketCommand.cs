using MediatR;

namespace TikiShop.Core.Services.BasketService.Commands;

public record AddToBasketCommand(int UserId, int ProductId) : IRequest<ServiceResult>;