namespace TikiShop.Core.Services.BasketService.Commands
{
    public record CreateBasketCommand(int UserId) : IRequest<ServiceResult>;
}
