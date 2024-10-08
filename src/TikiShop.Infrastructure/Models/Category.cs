using TikiShop.Infrastructure.Common;

namespace TikiShop.Infrastructure.Models
{
    public class Category : BaseEntity
    {
        public string Name { get; set; }
        public string? ThumbnailUrl { get; set; }
        public int? ParentId { get; set; }
        public Category? Parent { get; set; }
    }
}
