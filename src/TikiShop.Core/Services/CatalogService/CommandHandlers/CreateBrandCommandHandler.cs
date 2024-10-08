using MediatR;
using TikiShop.Core.RequestModels.Catalog;

namespace TikiShop.Core.Services.CatalogService.CommandHandlers
{
    internal class CreateBrandCommandHandler : IRequestHandler<CreateBrandRequest, ServiceResult>
    {
        public async Task<ServiceResult> Handle(CreateBrandRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
