namespace TikiShop.Core.Services.BasketService.Commands;

public record UpdateQuantityCommand(int UserId, int ProductSkuId, int Quantity) : IRequest<ServiceResult>;