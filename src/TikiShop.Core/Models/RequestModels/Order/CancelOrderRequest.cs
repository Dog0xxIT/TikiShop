using MediatR;
using TikiShop.Core.Services;

namespace TikiShop.Core.Models.RequestModels.Order
{
    public record CancelOrderRequest() : IRequest<ServiceResult>;
}
