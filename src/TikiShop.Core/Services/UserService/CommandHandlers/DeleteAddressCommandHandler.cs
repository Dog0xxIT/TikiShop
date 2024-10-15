using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TikiShop.Core.Services.UserService.Commands;
using TikiShop.Infrastructure;
using TikiShop.Infrastructure.Models;

namespace TikiShop.Core.Services.UserService.CommandHandlers
{
    internal class DeleteAddressCommandHandler : IRequestHandler<DeleteAddressCommand, ServiceResult>
    {
        private readonly TikiShopDbContext _context;
        private readonly ILogger<DeleteAddressCommand> _logger;

        public DeleteAddressCommandHandler(TikiShopDbContext context, ILogger<DeleteAddressCommand> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ServiceResult> Handle(DeleteAddressCommand request, CancellationToken cancellationToken)
        {
            var address = await _context.Addresses.FindAsync(request.Id);

            if (address is null)
            {
                return ServiceResult.Failed("Invalid Request");
            }

            address.IsDeleted = false;
            address.SetTimeLastModified();

            try
            {
                _context.Addresses.Update(address);
                await _context.SaveChangesAsync();
                return ServiceResult.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return ServiceResult.Failed();
            }
        }
    }
}
