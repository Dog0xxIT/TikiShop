using Catalog.Api.Data.Entities;

namespace TikiShop.Core.Models
{
    public class Brand : BaseEntity
    {
        public string Name { get; set; }
        public string Slug { get; set; }
    }
}
