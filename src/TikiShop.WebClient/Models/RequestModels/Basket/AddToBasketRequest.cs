using System.ComponentModel.DataAnnotations;

namespace TikiShop.WebClient.Models.RequestModels.Basket
{
    public class AddToBasketRequest
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        public int Quantity { get; set; }
    }
}
