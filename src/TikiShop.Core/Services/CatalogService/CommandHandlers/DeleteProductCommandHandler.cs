using MediatR;
using TikiShop.Core.RequestModels.Catalog;

namespace TikiShop.Core.Services.CatalogService.CommandHandlers
{
    internal class DeleteProductCommandHandler : IRequestHandler<DeleteProductRequest, ServiceResult>
    {
        public async Task<ServiceResult> Handle(DeleteProductRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
