using System;
using System.Threading;
using System.Threading.Tasks;
using TikiShop.Core.Services.UserService.Commands;
using TikiShop.Infrastructure;

namespace TikiShop.Core.Services.UserService.CommandHandlers
{
    internal class UpdateAddressCommandHandler : IRequestHandler<UpdateAddressCommand, ServiceResult>
    {
        private readonly TikiShopDbContext _context;
        private readonly ILogger<UpdateAddressCommandHandler> _logger; // Đổi tên cho logger

        public UpdateAddressCommandHandler(TikiShopDbContext context, ILogger<UpdateAddressCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ServiceResult> Handle(UpdateAddressCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Starting to update address with Id: {request.Id}");

            var address = await _context.Addresses.FindAsync(request.Id);

            if (address is null)
            {
                _logger.LogWarning($"Address not found for Id: {request.Id}");
                return ServiceResult.Failed("Invalid Request");
            }

            address.City = request.City;
            address.Country = request.Country;
            address.PhoneNumber = request.PhoneNumber;
            address.Receiver = request.Receiver;
            address.State = request.State;
            address.Street = request.Street;
            address.ZipCode = request.ZipCode;
            address.SetTimeLastModified();

            try
            {
                _context.Addresses.Update(address);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Address with Id: {request.Id} updated successfully.");
                return ServiceResult.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while updating address with Id: {request.Id}");
                return ServiceResult.Failed();
            }
        }
    }
}
