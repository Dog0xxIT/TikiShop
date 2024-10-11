using MediatR;
using TikiShop.Core.Models.RequestModels.Catalog;

namespace TikiShop.Core.Services.BasketService.CommandHandlers
{
    internal class UpdateQtyCommandHandler : IRequestHandler<UpdateQtyRequest, ServiceResult>
    {
        public async Task<ServiceResult> Handle(UpdateQtyRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
