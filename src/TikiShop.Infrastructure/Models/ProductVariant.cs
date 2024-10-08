using System.ComponentModel.DataAnnotations.Schema;
using TikiShop.Infrastructure.Common;

namespace TikiShop.Infrastructure.Models
{
    public class ProductVariant : BaseEntity
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Sku { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        [ForeignKey(nameof(OptionType1))]
        public int OptionTypeId1 { get; set; }
        public string OptionValue1 { get; set; }
        [ForeignKey(nameof(OptionType2))]
        public int? OptionTypeId2 { get; set; }
        public string? OptionValue2 { get; set; }

        public Product Product { get; set; }
        public OptionType OptionType1 { get; set; }
        public OptionType? OptionType2 { get; set; }
    }
}
