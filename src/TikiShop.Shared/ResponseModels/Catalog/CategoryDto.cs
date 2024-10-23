using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TikiShop.Shared.ResponseModels.Catalog
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ThumbnailUrl { get; set; }
        public int? ParentId { get; set; }
        public CategoryDto? Parent { get; set; }
        public bool? HasChild => Childs?.Any();
        public List<CategoryDto>? Childs { get; set; }
    }
}
