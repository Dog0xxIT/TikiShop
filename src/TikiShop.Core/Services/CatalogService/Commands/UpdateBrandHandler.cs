﻿using MediatR;
using TikiShop.Core.Models.RequestModels.Catalog;

namespace TikiShop.Core.Services.CatalogService.Commands;

public class UpdateBrandCommandHandler: IRequestHandler<UpdateBrandRequest, ServiceResult>
{
    public async Task<ServiceResult> Handle(UpdateBrandRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}