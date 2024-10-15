using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TikiShop.Infrastructure.Common;

namespace TikiShop.Infrastructure.Models
{
    public class PaymentProvider : BaseEntity
    {
        public string Name { get; set; }
    }
}
