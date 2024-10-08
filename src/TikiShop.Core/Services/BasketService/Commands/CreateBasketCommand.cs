using MediatR;

namespace TikiShop.Core.Services.BasketService.Commands;

public record CreateBasketCommand() : IRequest<ServiceResult>;