using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using TikiShop.Core.RequestModels.Basket;

namespace TikiShop.Core.Services.BasketService.CommandHandlers
{
    internal class AddToBasketCommandHandler : IRequestHandler<AddToBasketRequest, ServiceResult>
    {
        public async Task<ServiceResult> Handle(AddToBasketRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
