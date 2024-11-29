namespace TikiShop.Model.ResponseModels;

public sealed class PaginationResp<T> where T : class
{
    public PaginationMeta Meta { get; set; } = new();
    public IEnumerable<T> Data { get; set; } = Enumerable.Empty<T>();
}