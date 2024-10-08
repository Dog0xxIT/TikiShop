using MediatR;
using TikiShop.Core.Services.CatalogService.Commands;

namespace TikiShop.Core.Services.CatalogService.CommandHandlers;

public class UpdateBrandCommandHandler: IRequestHandler<UpdateBrandCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(UpdateBrandCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}