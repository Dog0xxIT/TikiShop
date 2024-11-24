namespace TikiShop.Model.DTO;

public record OrderItemDto(
    int ProductSkuId,
    int Quantity);