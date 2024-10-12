using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace TikiShop.Core.Services.BasketService.Commands
{
    public record CreateBasketCommand(int UserId) : IRequest<ServiceResult>;
}
