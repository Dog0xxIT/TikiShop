using TikiShop.Model.DTO;

namespace TikiShop.Core.Services.OrderService.Commands;

public record CancelOrderCommand(int OrderId, string Reason) : IRequest<ResultObject<int>>;

internal class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, ResultObject<int>>
{
    public async Task<ResultObject<int>> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}