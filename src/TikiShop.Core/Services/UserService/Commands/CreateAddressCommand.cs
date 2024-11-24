using TikiShop.Model.DTO;

namespace TikiShop.Core.Services.UserService.Commands;

public record CreateAddressCommand(
    string Receiver,
    string PhoneNumber,
    string Street,
    string City,
    string State,
    string Country,
    string ZipCode,
    int UserId) : IRequest<ResultObject<int>>;

internal class CreateAddressCommandHandler : IRequestHandler<CreateAddressCommand, ResultObject<int>>
{
    private readonly TikiShopDbContext _context;
    private readonly ILogService<CreateAddressCommandHandler> _logService; // Đổi tên cho _logService

    public CreateAddressCommandHandler(TikiShopDbContext context, ILogService<CreateAddressCommandHandler> logService)
    {
        _context = context;
        _logService = logService;
    }

    public async Task<ResultObject<int>> Handle(CreateAddressCommand request, CancellationToken cancellationToken)
    {
        _logService.LogInformation($"Starting to create address for Email: {request.UserId}");

        var isExistUser = await _context.Users.AnyAsync(u => u.Id == request.UserId);

        if (!isExistUser)
        {
            _logService.LogWarning($"User not found for Email: {request.UserId}");
            return ResultObject<int>.Failed("Invalid Request");
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
            _logService.LogInformation($"Address created successfully for Email: {request.UserId}");
            return ResultObject<int>.Success();
        }
        catch (Exception ex)
        {
            _logService.LogError(ex, $"Error occurred while creating address for Email: {request.UserId}");
            return ResultObject<int>.Failed("Error occurred while creating");
        }
    }
}