using MediatR;
using TikiShop.Core.Services;

namespace TikiShop.Core.RequestModels.Order
{
    public record CancelOrderRequest() : IRequest<ServiceResult>;
}
