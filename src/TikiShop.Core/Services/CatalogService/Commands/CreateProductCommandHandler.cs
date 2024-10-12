using MediatR;
using TikiShop.Core.Models.RequestModels.Catalog;

namespace TikiShop.Core.Services.CatalogService.Commands
{
    internal class CreateProductCommandHandler : IRequestHandler<CreateProductRequest, ServiceResult>
    {
        public async Task<ServiceResult> Handle(CreateProductRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
