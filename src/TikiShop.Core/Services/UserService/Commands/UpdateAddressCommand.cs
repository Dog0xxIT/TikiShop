using TikiShop.Model.DTO;

namespace TikiShop.Core.Services.UserService.Commands;

public record UpdateAddressCommand(
    int Id,
    string Receiver,
    string PhoneNumber,
    string Street,
    string City,
    string State,
    string Country,
    string ZipCode) : IRequest<ResultObject<int>>;

internal class UpdateAddressCommandHandler : IRequestHandler<UpdateAddressCommand, ResultObject<int>>
{
    private readonly TikiShopDbContext _context;
    private readonly ILogService<UpdateAddressCommandHandler> _logService;

    public UpdateAddressCommandHandler(TikiShopDbContext context, ILogService<UpdateAddressCommandHandler> logService)
    {
        _context = context;
        _logService = logService;
    }

    public async Task<ResultObject<int>> Handle(UpdateAddressCommand request, CancellationToken cancellationToken)
    {
        _logService.LogInformation($"Starting to update address with Id: {request.Id}");

        var address = await _context.Addresses.FindAsync(request.Id);

        if (address is null)
        {
            _logService.LogWarning($"Address not found for Id: {request.Id}");
            return ResultObject<int>.Failed("Invalid Request");
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
            _logService.LogInformation($"Address with Id: {request.Id} updated successfully.");
            return ResultObject<int>.Success();
        }
        catch (Exception ex)
        {
            _logService.LogError(ex, $"Error occurred while updating address with Id: {request.Id}");
            return ResultObject<int>.Failed("Error occurred while updating address");
        }
    }
}