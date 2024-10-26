using System;
using System.Threading;
using System.Threading.Tasks;
using TikiShop.Core.Services.UserService.Commands;
using TikiShop.Infrastructure;
using TikiShop.Infrastructure.Models;

namespace TikiShop.Core.Services.UserService.CommandHandlers
{
    internal class DeleteAddressCommandHandler : IRequestHandler<DeleteAddressCommand, ServiceResult>
    {
        private readonly TikiShopDbContext _context;
        private readonly ILogger<DeleteAddressCommandHandler> _logger; // Đổi tên cho logger

        public DeleteAddressCommandHandler(TikiShopDbContext context, ILogger<DeleteAddressCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ServiceResult> Handle(DeleteAddressCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Starting to delete address with Id: {request.Id}");

            var address = await _context.Addresses.FindAsync(request.Id);

            if (address is null)
            {
                _logger.LogWarning($"Address not found for Id: {request.Id}");
                return ServiceResult.Failed("Invalid Request");
            }

            address.IsDeleted = true;
            address.SetTimeLastModified();

            try
            {
                _context.Addresses.Update(address);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Address with Id: {request.Id} deleted successfully.");
                return ServiceResult.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while deleting address with Id: {request.Id}");
                return ServiceResult.Failed();
            }
        }
    }
}