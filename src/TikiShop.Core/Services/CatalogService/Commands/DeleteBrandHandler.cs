using MediatR;
using TikiShop.Core.Models.RequestModels.Catalog;

namespace TikiShop.Core.Services.CatalogService.Commands
{
    internal class DeleteBrandHandler : IRequestHandler<DeleteBrandRequest, ServiceResult>
    {
        public async Task<ServiceResult> Handle(DeleteBrandRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
