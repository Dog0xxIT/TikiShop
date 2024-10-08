using MediatR;
using TikiShop.Core.Services;

namespace TikiShop.Core.RequestModels.Basket;

public record CreateBasketRequest() : IRequest<ServiceResult>;