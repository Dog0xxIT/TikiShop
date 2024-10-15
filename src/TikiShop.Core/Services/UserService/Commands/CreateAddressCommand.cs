namespace TikiShop.Core.Services.UserService.Commands;

public record CreateAddressCommand(
     string Receiver,
     string PhoneNumber,
     string Street,
     string City,
     string State,
     string Country,
     string ZipCode,
     int UserId) : IRequest<ServiceResult>;