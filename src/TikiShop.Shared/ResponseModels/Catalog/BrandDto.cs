using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TikiShop.Shared.ResponseModels.Catalog
{
    public class BrandDto
    {  
        public int Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
    }
}
