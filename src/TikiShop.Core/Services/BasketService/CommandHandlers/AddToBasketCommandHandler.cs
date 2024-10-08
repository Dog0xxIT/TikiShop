using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using TikiShop.Core.Services.BasketService.Commands;

namespace TikiShop.Core.Services.BasketService.CommandHandlers
{
    internal class AddToBasketCommandHandler : IRequestHandler<AddToBasketCommand, ServiceResult>
    {
        public async Task<ServiceResult> Handle(AddToBasketCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
