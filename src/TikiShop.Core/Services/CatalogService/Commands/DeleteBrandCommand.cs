namespace TikiShop.Core.Services.CatalogService.Commands;

public record DeleteBrandCommand(int Id) : IRequest<ServiceResult>;