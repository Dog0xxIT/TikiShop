using MediatR;

namespace TikiShop.Core.Services.CatalogService.Commands;

public record UpdateBrandCommand() : IRequest<ServiceResult>;