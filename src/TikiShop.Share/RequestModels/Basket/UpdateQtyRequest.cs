using System.ComponentModel.DataAnnotations;

namespace TikiShop.Share.RequestModels.Basket
{
    public sealed class UpdateQtyRequest
    {
        public int BasketItemId { get; set; }

        [Range(0, Int32.MaxValue)]
        public int Qty { get; set; }
    }
}
