namespace TikiShop.Model.ResponseModels;

public sealed class PaginationMeta
{
    /// <summary>
    ///     Total number of items in the result set.
    /// </summary>
    public int Total { get; set; }

    /// <summary>
    ///     Total number of items in the collection response.
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    ///     The amount of items returned in the collection per page, controlled by the limit parameter.
    /// </summary>
    public int PerPage { get; set; }

    /// <summary>
    ///     The page you are currently on within the collection.
    ///     <example>
    ///         Example: 1
    ///     </example>
    /// </summary>
    public int CurrentPage { get; set; }

    /// <summary>
    ///     The total number of pages in the collection.
    /// </summary>
    public int TotalPages { get; set; }
}