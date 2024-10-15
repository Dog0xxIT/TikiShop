using TikiShop.Infrastructure.Models;

namespace TikiShop.Core.Services.CatalogService.Commands;

public record CreateProductCommand(
    string Sku,
    string Name,
    string? Description,
    string? Summary,
    double Price,
    string? ThumbnailUrl,
    int Quantity,
    int CategoryId,
    int BrandId) : IRequest<ServiceResult>;