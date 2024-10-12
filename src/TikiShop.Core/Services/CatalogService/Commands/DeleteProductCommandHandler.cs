using MediatR;
using TikiShop.Core.Models.RequestModels.Catalog;

namespace TikiShop.Core.Services.CatalogService.Commands
{
    internal class DeleteProductCommandHandler : IRequestHandler<DeleteProductRequest, ServiceResult>
    {
        public async Task<ServiceResult> Handle(DeleteProductRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
