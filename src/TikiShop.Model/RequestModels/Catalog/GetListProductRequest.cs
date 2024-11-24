using System.ComponentModel.DataAnnotations;
using TikiShop.Model.Enums;

namespace TikiShop.Model.RequestModels.Catalog;

public class GetListProductRequest : PaginationRequest
{
    [Range(0.001, double.MaxValue)] public double? MinPrice { get; set; }

    [Range(0.001, double.MaxValue)] public double? MaxPrice { get; set; }

    [RegularExpression(@"^(\d+)(\,\d+)*$", ErrorMessage = "A comma-separated list of brand IDs")]
    public string? Brands { get; set; }

    public string? Keyword { get; set; }

    [Range(1, 5)] public int? Rating { get; set; }

    [RegularExpression(@"^(\d+)(\,\d+)*$", ErrorMessage = "A comma-separated list of category IDs")]
    public string? Categories { get; set; }

    public SortBy SortBy { get; set; } = SortBy.Id;
}