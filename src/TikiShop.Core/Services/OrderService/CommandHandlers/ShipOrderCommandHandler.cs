using MediatR;
using TikiShop.Core.Services.OrderService.Commands;

namespace TikiShop.Core.Services.OrderService.CommandHandlers;

internal class ShipOrderCommandHandler : IRequestHandler<ShipOrderCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(ShipOrderCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
