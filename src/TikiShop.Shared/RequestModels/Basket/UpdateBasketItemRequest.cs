using System.ComponentModel.DataAnnotations;

namespace TikiShop.Shared.RequestModels.Basket
{
    public class UpdateBasketItemRequest
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, 1000)]
        public int Quantity { get; set; }
    }
}
