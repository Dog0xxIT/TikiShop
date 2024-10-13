namespace TikiShop.Core.Services.BasketService.Commands;

public record UpdateQuantityCommand(int UserId, int ProductId, int? ProductVariantId, int Quantity) : IRequest<ServiceResult>;