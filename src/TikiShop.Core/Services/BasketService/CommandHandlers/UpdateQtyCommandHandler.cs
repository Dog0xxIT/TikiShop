using MediatR;
using TikiShop.Core.Services.BasketService.Commands;

namespace TikiShop.Core.Services.BasketService.CommandHandlers
{
    internal class UpdateQtyCommandHandler : IRequestHandler<UpdateQtyCommand, ServiceResult>
    {
        public async Task<ServiceResult> Handle(UpdateQtyCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
