using MediatR;
using TikiShop.Core.Services.BasketService.Commands;

namespace TikiShop.Core.Services.BasketService.CommandHandlers;

internal class CreateBasketCommandHandler  : IRequestHandler<CreateBasketCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(CreateBasketCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}