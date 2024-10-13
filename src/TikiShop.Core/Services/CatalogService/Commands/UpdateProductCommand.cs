namespace TikiShop.Core.Services.CatalogService.Commands
{
    public record UpdateProductCommand(
        int Id,
        string Sku,
        string Name,
        string? Description,
        string? ShortDescription,
        double Price,
        string? ThumbnailUrl,
        int Quantity,
        int CategoryId,
        int BrandId) : IRequest<ServiceResult>;
}
