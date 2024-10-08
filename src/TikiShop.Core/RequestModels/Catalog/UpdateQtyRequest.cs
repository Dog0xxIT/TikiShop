using MediatR;
using TikiShop.Core.Services;

namespace TikiShop.Core.RequestModels.Catalog
{
    public record UpdateQtyRequest() : IRequest<ServiceResult>;
}
