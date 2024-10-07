using Catalog.Api.Data.Entities;

namespace TikiShop.Core.Models
{
    public class OptionType : BaseEntity
    {
        public string Name { get; set; }
        public string Code { get; set; }
    }
}
