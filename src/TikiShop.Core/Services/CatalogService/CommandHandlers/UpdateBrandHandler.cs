using MediatR;
using TikiShop.Core.RequestModels.Catalog;

namespace TikiShop.Core.Services.CatalogService.CommandHandlers;

public class UpdateBrandCommandHandler: IRequestHandler<UpdateBrandRequest, ServiceResult>
{
    public async Task<ServiceResult> Handle(UpdateBrandRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}