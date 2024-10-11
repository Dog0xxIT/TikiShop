using MediatR;
using System.ComponentModel.DataAnnotations;
using TikiShop.Core.Services;

namespace TikiShop.Core.Models.RequestModels.Basket
{
    public class AddToBasketRequest : IRequest<ServiceResult>
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        public int Quantity { get; set; }
    }
}
