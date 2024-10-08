using TikiShop.Infrastructure.Common;

namespace TikiShop.Infrastructure.Models
{
    public class OptionType : BaseEntity
    {
        public string Name { get; set; }
        public string Code { get; set; }
    }
}
