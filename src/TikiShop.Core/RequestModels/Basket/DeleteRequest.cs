using MediatR;
using TikiShop.Core.Services;

namespace TikiShop.Core.RequestModels.Basket
{
    public record DeleteRequest : IRequest<ServiceResult>;
}
