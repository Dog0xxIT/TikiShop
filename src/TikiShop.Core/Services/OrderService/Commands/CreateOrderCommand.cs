using TikiShop.Core.Dto;

namespace TikiShop.Core.Services.OrderService.Commands;

public record CreateOrderCommand(
    int BuyerId,
    int AddressId,
    List<OrderItemDto> Items) : IRequest<ServiceResult>;