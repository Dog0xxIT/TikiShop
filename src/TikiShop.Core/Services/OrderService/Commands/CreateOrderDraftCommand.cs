using MediatR;

namespace TikiShop.Core.Services.OrderService.Commands;

public record CreateOrderDraftCommand() : IRequest<ServiceResult>;