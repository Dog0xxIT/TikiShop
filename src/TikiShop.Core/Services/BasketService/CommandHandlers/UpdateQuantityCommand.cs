using MediatR;

namespace TikiShop.Core.Services.BasketService.CommandHandlers;

public record UpdateQuantityCommand(int UserId, int ProductId, int Quantity) : IRequest<ServiceResult>;