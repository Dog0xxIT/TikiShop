using MediatR;
using TikiShop.Core.Models.RequestModels.Order;

namespace TikiShop.Core.Services.OrderService.CommandHandlers
{
    internal class CancelOrderCommandHandler : IRequestHandler<CancelOrderRequest, ServiceResult>
    {
        public async Task<ServiceResult> Handle(CancelOrderRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
