using TikiShop.Core.Services.OrderService.Commands;

namespace TikiShop.Core.Services.OrderService.CommandHandlers
{
    internal class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, ServiceResult>
    {
        public async Task<ServiceResult> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
