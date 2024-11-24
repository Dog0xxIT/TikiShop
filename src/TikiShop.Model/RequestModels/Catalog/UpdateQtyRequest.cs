using System.ComponentModel.DataAnnotations;

namespace TikiShop.Model.RequestModels.Catalog;

public class UpdateQtyRequest
{
    [Required] public int BasketItemId { get; set; }

    [Required] public int Quantity { get; set; }
}