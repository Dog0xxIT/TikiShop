using MediatR;
using TikiShop.Core.RequestModels.Order;

namespace TikiShop.Core.Services.OrderService.CommandHandlers
{
    internal class CreateOrderCommandHandler : IRequestHandler<CreateOrderRequest, ServiceResult>
    {
        public async Task<ServiceResult> Handle(CreateOrderRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
