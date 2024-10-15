using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TikiShop.Infrastructure.Common;

namespace TikiShop.Infrastructure.Models
{
    public class ProductAttribute : BaseEntity
    {
        public string Type { get; set; }
        public string Value { get; set; }
        public string Code { get; set; }
    }
}
