namespace TikiShop.Core.Services.CatalogService.Commands;

public record DeleteProductCommand(int Id) : IRequest<ServiceResult>;