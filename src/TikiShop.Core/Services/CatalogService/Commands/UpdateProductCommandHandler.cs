using MediatR;
using TikiShop.Core.Models.RequestModels.Catalog;

namespace TikiShop.Core.Services.CatalogService.Commands
{
    internal class UpdateProductCommandHandler : IRequestHandler<UpdateProductRequest, ServiceResult>
    {
        public async Task<ServiceResult> Handle(UpdateProductRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
