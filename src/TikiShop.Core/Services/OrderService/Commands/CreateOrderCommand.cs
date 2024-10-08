using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TikiShop.Core.Services.OrderService.Commands
{
    public record CreateOrderCommand () : IRequest<ServiceResult>;
}
