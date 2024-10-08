using MediatR;
using TikiShop.Core.RequestModels.Catalog;

namespace TikiShop.Core.Services.CatalogService.CommandHandlers
{
    internal class DeleteBrandHandler : IRequestHandler<DeleteBrandRequest, ServiceResult>
    {
        public async Task<ServiceResult> Handle(DeleteBrandRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
