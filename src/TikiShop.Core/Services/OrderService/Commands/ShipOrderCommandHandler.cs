using MediatR;
using TikiShop.Core.Models.RequestModels.Order;

namespace TikiShop.Core.Services.OrderService.Commands;

internal class ShipOrderCommandHandler : IRequestHandler<ShipOrderRequest, ServiceResult>
{
    public async Task<ServiceResult> Handle(ShipOrderRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
