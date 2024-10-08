using MediatR;
using TikiShop.Core.Services;

namespace TikiShop.Core.RequestModels.Catalog
{
    public record CreateBrandRequest : IRequest<ServiceResult>;
}
