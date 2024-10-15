using TikiShop.Core.Services.UserService.Commands;
using TikiShop.Infrastructure;
using TikiShop.Infrastructure.Models;

namespace TikiShop.Core.Services.UserService.CommandHandlers
{
    internal class CreateAddressCommandHandler : IRequestHandler<CreateAddressCommand, ServiceResult>
    {
        private readonly TikiShopDbContext _context;
        private readonly ILogger<CreateAddressCommand> _logger;

        public CreateAddressCommandHandler(TikiShopDbContext context, ILogger<CreateAddressCommand> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ServiceResult> Handle(CreateAddressCommand request, CancellationToken cancellationToken)
        {
            var isExistUser = await _context.Users.AnyAsync(u => u.Id == request.UserId);

            if (!isExistUser)
            {
                return ServiceResult.Failed("Invalid Request");
            }

            var address = new Address
            {
                City = request.City,
                Country = request.Country,
                PhoneNumber = request.PhoneNumber,
                Receiver = request.Receiver,
                State = request.State,
                Street = request.Street,
                UserId = request.UserId,
                ZipCode = request.ZipCode
            };

            try
            {
                await _context.Addresses.AddAsync(address);
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
