using System.ComponentModel.DataAnnotations;

namespace TikiShop.Model.RequestModels.Basket;

public class UpdateBasketItemReq
{
    [Required] public int ProductId { get; set; }

    [Required] [Range(1, 1000)] public int Quantity { get; set; }
}