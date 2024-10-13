namespace TikiShop.Core.Services.CatalogService.Commands
{
    public record CreateBrandCommand(string Name) : IRequest<ServiceResult>;
}
