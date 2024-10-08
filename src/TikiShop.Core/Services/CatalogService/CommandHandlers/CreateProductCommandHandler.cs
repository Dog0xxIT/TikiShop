using MediatR;
using TikiShop.Core.Services.CatalogService.Commands;

namespace TikiShop.Core.Services.CatalogService.CommandHandlers
{
    internal class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ServiceResult>
    {
        public async Task<ServiceResult> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
