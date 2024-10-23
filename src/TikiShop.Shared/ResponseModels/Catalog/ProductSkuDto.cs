using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TikiShop.Shared.ResponseModels.Catalog
{
    public class ProductSkuDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Sku { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public ConfigOptionDto? Option1 { get; set; }
        public ConfigOptionDto? Option2 { get; set; }
    }
}
