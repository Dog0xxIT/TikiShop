using MediatR;
using TikiShop.Core.RequestModels.Order;

namespace TikiShop.Core.Services.OrderService.CommandHandlers;

internal class ShipOrderCommandHandler : IRequestHandler<ShipOrderRequest, ServiceResult>
{
    public async Task<ServiceResult> Handle(ShipOrderRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
