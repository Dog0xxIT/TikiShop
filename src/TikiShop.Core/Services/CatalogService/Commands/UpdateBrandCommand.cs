namespace TikiShop.Core.Services.CatalogService.Commands;

public record UpdateBrandCommand(int Id, string Name) : IRequest<ServiceResult>;