using MediatR;
using TikiShop.Core.Services.CatalogService.Commands;

namespace TikiShop.Core.Services.CatalogService.CommandHandlers
{
    internal class CreateBrandCommandHandler : IRequestHandler<CreateBrandCommand, ServiceResult>
    {
        public async Task<ServiceResult> Handle(CreateBrandCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
