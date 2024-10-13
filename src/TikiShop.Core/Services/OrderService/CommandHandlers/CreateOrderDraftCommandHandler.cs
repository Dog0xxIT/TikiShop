using TikiShop.Core.Services.OrderService.Commands;

namespace TikiShop.Core.Services.OrderService.CommandHandlers;

public class CreateOrderDraftCommandHandler : IRequestHandler<CreateOrderDraftCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(CreateOrderDraftCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}