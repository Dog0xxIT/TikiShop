using MediatR;
using TikiShop.Core.Services.CatalogService.Commands;

namespace TikiShop.Core.Services.CatalogService.CommandHandlers
{
    internal class DeleteBrandHandler : IRequestHandler<DeleteBrand, ServiceResult>
    {
        public async Task<ServiceResult> Handle(DeleteBrand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
