using MediatR;
using TikiShop.Core.Models.RequestModels.Order;

namespace TikiShop.Core.Services.OrderService.CommandHandlers;

public class CreateOrderDraftCommandHandler : IRequestHandler<CreateOrderDraftRequest, ServiceResult>
{
    public async Task<ServiceResult> Handle(CreateOrderDraftRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}