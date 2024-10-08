using MediatR;
using TikiShop.Core.RequestModels.Basket;

namespace TikiShop.Core.Services.BasketService.CommandHandlers
{
    internal class DeleteCommandHandler  : IRequestHandler<DeleteRequest, ServiceResult>
    {
        public async Task<ServiceResult> Handle(DeleteRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
