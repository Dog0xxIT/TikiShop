using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TikiShop.Domain.Commands;

namespace TikiShop.Domain.CommandHandlers
{
    public class CreateOrderDraftCommandHandler : IRequestHandler<CreateOrderDraftCommand, bool>
    {
        private ILogger<CreateOrderDraftCommandHandler> _logger;

        public CreateOrderDraftCommandHandler(ILogger<CreateOrderDraftCommandHandler> logger)
        {
            _logger = logger;
        }

        public async Task<bool> Handle(CreateOrderDraftCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
