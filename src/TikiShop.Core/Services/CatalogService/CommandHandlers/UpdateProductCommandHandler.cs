using MediatR;
using TikiShop.Core.Services.CatalogService.Commands;

namespace TikiShop.Core.Services.CatalogService.CommandHandlers
{
    internal class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ServiceResult>
    {
        public async Task<ServiceResult> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
