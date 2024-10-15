namespace TikiShop.Core.Services.UserService.Commands;

public record DeleteAddressCommand(int Id) : IRequest<ServiceResult>;