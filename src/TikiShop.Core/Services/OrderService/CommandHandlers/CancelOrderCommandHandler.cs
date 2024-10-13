using TikiShop.Core.Services.OrderService.Commands;

namespace TikiShop.Core.Services.OrderService.CommandHandlers
{
    internal class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, ServiceResult>
    {
        public async Task<ServiceResult> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
