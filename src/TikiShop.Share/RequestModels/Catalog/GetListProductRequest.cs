using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TikiShop.Share.RequestModels.Catalog
{
    public class GetListProductRequest : PaginationRequest
    {
        [Range(0.001, double.MaxValue)]
        public double? MinPrice { get; set; }

        [Range(0.001, double.MaxValue)]
        public double? MaxPrice { get; set; }

        [RegularExpression(@"^(\d+)(\,\d+)*$", ErrorMessage = "A comma-separated list of brand IDs")]
        public string? Brands { get; set; }

        public string? Keyword { get; set; }

        public int? Rating { get; set; }

        [RegularExpression(@"^(\d+)(\,\d+)*$", ErrorMessage = "A comma-separated list of category IDs")]
        public string? Categories { get; set; }

        [RegularExpression(@"^(id|price|date_modified)$", ErrorMessage = "Sort direction. Acceptable values are: id, price, date_modified.")]
        public string SortBy { get; set; } = "id";
    }
}
