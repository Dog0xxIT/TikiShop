namespace TikiShop.Model.ResponseModels.Catalog;

public class GetListCategoriesResp
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string ThumbnailUrl { get; set; }

    public int? ParentId { get; set; }

    public bool HasChild => Child.Any();

    public List<GetListCategoriesResp> Child { get; set; } = new();
}