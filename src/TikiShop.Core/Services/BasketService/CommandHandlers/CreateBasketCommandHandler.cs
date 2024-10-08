using MediatR;
using TikiShop.Core.RequestModels.Basket;

namespace TikiShop.Core.Services.BasketService.CommandHandlers;

internal class CreateBasketCommandHandler  : IRequestHandler<CreateBasketRequest, ServiceResult>
{
    public async Task<ServiceResult> Handle(CreateBasketRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}