using System.ComponentModel.DataAnnotations;

namespace TikiShop.Shared.RequestModels.Catalog
{
    public class UpdateQtyRequest
    {
        [Required]
        public int BasketItemId { get; set; }

        [Required]
        public int Quantity { get; set; }
    }
}
