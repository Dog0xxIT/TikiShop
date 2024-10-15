using TikiShop.Core.Services.UserService.Commands;
using TikiShop.Infrastructure;

namespace TikiShop.Core.Services.UserService.CommandHandlers
{
    internal class UpdateAddressCommandHandler : IRequestHandler<UpdateAddressCommand, ServiceResult>
    {
        private readonly TikiShopDbContext _context;
        private readonly ILogger<UpdateAddressCommand> _logger;

        public UpdateAddressCommandHandler(TikiShopDbContext context, ILogger<UpdateAddressCommand> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ServiceResult> Handle(UpdateAddressCommand request, CancellationToken cancellationToken)
        {
            var address = await _context.Addresses.FindAsync(request.Id);

            if (address is null)
            {
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
