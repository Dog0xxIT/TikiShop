using MediatR;
using Microsoft.Extensions.Logging;

namespace TikiShop.Domain.Commands
{
    public record CreateOrderDraftCommand() : IRequest<bool>;

    
}
