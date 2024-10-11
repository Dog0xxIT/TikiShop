using MediatR;
using TikiShop.Core.Services;

namespace TikiShop.Core.Models.RequestModels.Basket
{
    public record DeleteRequest : IRequest<ServiceResult>;
}
