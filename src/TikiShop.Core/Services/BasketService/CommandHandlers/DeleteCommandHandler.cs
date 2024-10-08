using MediatR;
using TikiShop.Core.Services.BasketService.Commands;

namespace TikiShop.Core.Services.BasketService.CommandHandlers
{
    internal class DeleteCommandHandler  : IRequestHandler<DeleteCommand, ServiceResult>
    {
        public async Task<ServiceResult> Handle(DeleteCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
