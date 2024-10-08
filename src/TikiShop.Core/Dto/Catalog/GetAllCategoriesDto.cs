namespace TikiShop.Core.Dto.Catalog
{
    public class GetAllCategoriesDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string ThumbnailUrl { get; set; }

        public int? ParentId { get; set; }

        public bool HasChild => Childs.Any();

        public List<GetAllCategoriesDto> Childs { get; set; } = new();
    }
}
