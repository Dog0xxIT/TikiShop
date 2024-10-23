using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TikiShop.Shared.ResponseModels.Catalog
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double? Price { get; set; }
        public double Discount { get; set; }
        public double RatingAverage { get; set; }
        public int ReviewCount { get; set; }
        public string ThumbnailUrl { get; set; }
        public CategoryDto? Category { get; set; }
        public BrandDto? Brand { get; set; }
        public int TotalBought { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public List<ProductSkuDto>? ProductSkus { get; set; }
    }




}
