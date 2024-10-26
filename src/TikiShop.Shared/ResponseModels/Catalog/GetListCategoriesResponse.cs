namespace TikiShop.Shared.ResponseModels.Catalog
{
    public class GetListCategoriesResponse
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string ThumbnailUrl { get; set; }

        public int? ParentId { get; set; }

        public bool HasChild => Child.Any();

        public List<GetListCategoriesResponse> Child { get; set; } = new();
    }
}
