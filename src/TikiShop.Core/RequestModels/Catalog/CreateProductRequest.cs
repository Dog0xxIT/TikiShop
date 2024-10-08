using MediatR;
using TikiShop.Core.Services;

namespace TikiShop.Core.RequestModels.Catalog
{
    public record CreateProductRequest : IRequest<ServiceResult>;
}
