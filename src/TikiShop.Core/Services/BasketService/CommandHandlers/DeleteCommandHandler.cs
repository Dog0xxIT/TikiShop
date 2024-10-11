using MediatR;
using TikiShop.Core.Models.RequestModels.Basket;

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
