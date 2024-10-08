using MediatR;
using TikiShop.Core.Services;

namespace TikiShop.Core.RequestModels.Order;

public record CreateOrderDraftRequest() : IRequest<ServiceResult>;