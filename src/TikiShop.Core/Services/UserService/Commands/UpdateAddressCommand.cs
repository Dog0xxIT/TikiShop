namespace TikiShop.Core.Services.UserService.Commands;

public record UpdateAddressCommand(
    int Id,
    string Receiver,
    string PhoneNumber,
    string Street,
    string City,
    string State,
    string Country,
    string ZipCode) : IRequest<ServiceResult>;