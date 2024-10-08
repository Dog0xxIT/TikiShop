namespace TikiShop.Core.Dto
{
    public sealed class PaginationDto<T> where T : class
    {
        public PaginationMetaDto Meta { get; set; } = new();
        public IEnumerable<T> Data { get; set; } = Enumerable.Empty<T>();
    }
}
