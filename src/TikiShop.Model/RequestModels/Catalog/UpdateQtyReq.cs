using System.ComponentModel.DataAnnotations;

namespace TikiShop.Model.RequestModels.Catalog;

public class UpdateQtyReq
{
    [Required] public int BasketItemId { get; set; }

    [Required] public int Quantity { get; set; }
}