using System.ComponentModel.DataAnnotations;

namespace TikiShop.Share.RequestModels.Basket
{
    public class AddToBasketRequest
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        public int Quantity { get; set; }
    }
}
