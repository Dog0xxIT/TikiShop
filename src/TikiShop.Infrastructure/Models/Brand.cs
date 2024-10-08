using TikiShop.Infrastructure.Common;

namespace TikiShop.Infrastructure.Models
{
    public class Brand : BaseEntity
    {
        public string Name { get; set; }
        public string Slug { get; set; }
    }
}
