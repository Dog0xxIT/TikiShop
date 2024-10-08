using MediatR;
using TikiShop.Core.Services.CatalogService.Commands;

namespace TikiShop.Core.Services.CatalogService.CommandHandlers
{
    internal class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, ServiceResult>
    {
        public async Task<ServiceResult> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
