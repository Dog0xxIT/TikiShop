using TikiShop.Model.DTO;

namespace TikiShop.Core.Services.OrderService.Commands;

public record ShipOrderCommand : IRequest<ResultObject<int>>;

internal class ShipOrderCommandHandler : IRequestHandler<ShipOrderCommand, ResultObject<int>>
{
    public async Task<ResultObject<int>> Handle(ShipOrderCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}