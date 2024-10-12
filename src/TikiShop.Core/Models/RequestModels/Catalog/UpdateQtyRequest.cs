using System.ComponentModel.DataAnnotations;
using MediatR;
using TikiShop.Core.Services;

namespace TikiShop.Core.Models.RequestModels.Catalog
{
    public class UpdateQtyRequest
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        public int Quantity { get; set; }
    }
}
