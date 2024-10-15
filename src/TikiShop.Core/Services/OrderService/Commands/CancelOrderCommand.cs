namespace TikiShop.Core.Services.OrderService.Commands;

public record CancelOrderCommand(int OrderId, string Reason) : IRequest<ServiceResult>;