using MediatR;
using TikiShop.Core.Services;

namespace TikiShop.Core.Models.RequestModels.Catalog
{
    public record DeleteBrandRequest : IRequest<ServiceResult>;
}
