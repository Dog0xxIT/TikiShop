using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TikiShop.Infrastructure.Common;

namespace TikiShop.Infrastructure.Models
{
    public class Review : BaseEntity
    {
        public string RoomId { get; set; }
        public Order Order { get; set; }
    }
}
