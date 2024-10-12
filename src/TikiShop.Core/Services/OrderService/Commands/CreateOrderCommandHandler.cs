using MediatR;
using TikiShop.Core.Models.RequestModels.Order;

namespace TikiShop.Core.Services.OrderService.Commands
{
    internal class CreateOrderCommandHandler : IRequestHandler<CreateOrderRequest, ServiceResult>
    {
        public async Task<ServiceResult> Handle(CreateOrderRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
