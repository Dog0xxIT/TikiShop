namespace TikiShop.Shared.ResponseModels
{
    public sealed class PaginationResponse<T> where T : class
    {
        public PaginationMetaDto Meta { get; set; } = new();
        public IEnumerable<T> Data { get; set; } = Enumerable.Empty<T>();
    }
}
