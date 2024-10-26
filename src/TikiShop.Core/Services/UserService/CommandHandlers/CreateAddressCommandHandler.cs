using TikiShop.Core.Services.UserService.Commands;
using TikiShop.Infrastructure;
using TikiShop.Infrastructure.Models;

namespace TikiShop.Core.Services.UserService.CommandHandlers
{
    internal class CreateAddressCommandHandler : IRequestHandler<CreateAddressCommand, ServiceResult>
    {
        private readonly TikiShopDbContext _context;
        private readonly ILogger<CreateAddressCommandHandler> _logger; // Đổi tên cho logger

        public CreateAddressCommandHandler(TikiShopDbContext context, ILogger<CreateAddressCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ServiceResult> Handle(CreateAddressCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Starting to create address for UserId: {request.UserId}");

            var isExistUser = await _context.Users.AnyAsync(u => u.Id == request.UserId);

            if (!isExistUser)
            {
                _logger.LogWarning($"User not found for UserId: {request.UserId}");
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
                _logger.LogInformation($"Address created successfully for UserId: { request.UserId}");
                return ServiceResult.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while creating address for UserId: { request.UserId}");
                return ServiceResult.Failed();
            }
        }
    }
}
