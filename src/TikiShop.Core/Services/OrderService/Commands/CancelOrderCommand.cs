namespace TikiShop.Core.Services.OrderService.Commands;

public record CancelOrderCommand() : IRequest<ServiceResult>;