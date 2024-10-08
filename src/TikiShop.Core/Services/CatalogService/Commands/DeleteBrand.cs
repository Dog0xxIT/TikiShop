using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TikiShop.Core.Services.CatalogService.Commands
{
    public record DeleteBrand : IRequest<ServiceResult>;
}
