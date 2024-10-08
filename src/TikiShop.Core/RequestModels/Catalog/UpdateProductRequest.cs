using MediatR;
using TikiShop.Core.Services;

namespace TikiShop.Core.RequestModels.Catalog
{
    public record UpdateProductRequest : IRequest<ServiceResult>;
}
