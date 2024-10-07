using MediatR;
using Microsoft.AspNetCore.Mvc;
using TikiShop.Core.Services.CatalogService.Queries;
using TikiShop.Share.RequestModels;
using TikiShop.Share.RequestModels.Catalog;

namespace TikiShop.Api.Controllers;

[ApiController]
[Route("/api/v1/catalog")]
public class CatalogController : Controller
{
    private readonly IMediator _mediator;
    private readonly ICatalogQueries _catalogQueries;

    public CatalogController(IMediator mediator, ICatalogQueries catalogQueries)
    {
        _mediator = mediator;
        _catalogQueries = catalogQueries;
    }

    [HttpGet("products")]
    public async Task<IActionResult> GetListProducts([FromQuery] GetListProductRequest req)
    {
        return Ok();
    }

    [HttpGet("products/{id}")]
    public async Task<IActionResult> GetProductById([FromRoute] int id)
    {
        return Ok();
    }

    [HttpPost("products")]
    public async Task<IActionResult> CreateProduct(CreateProductRequest req)
    {
        return Ok();
    }

    [HttpPut("products/{id}")]
    public async Task<IActionResult> UpdateProduct(
        [FromRoute] int id,
        [FromBody] UpdateProductRequest req)
    {
        return Ok();
    }

    [HttpDelete("products/{id}")]
    public async Task<IActionResult> DeleteProduct([FromRoute] int id)
    {
        return Ok();
    }

    [HttpGet("brands")]
    public async Task<IActionResult> GetListBrands([FromQuery] PaginationRequest paginationReq)
    {
        return Ok();
    }

    [HttpPost("brands")]
    public async Task<IActionResult> CreateBrand(CreateBrandRequest req)
    {
        return Ok();
    }

    [HttpPut("brands/{id}")]
    public async Task<IActionResult> UpdateBrand(
        [FromRoute] int id,
        [FromBody] UpdateBrandRequest req)
    {
        return Ok();
    }

    [HttpDelete("brands/{id}")]
    public async Task<IActionResult> DeleteBrand([FromRoute] int id)
    {
        return Ok();

    }

    [HttpGet("categories/hierarchy")]
    public async Task<IActionResult> GetCategoriesHierarchy()
    {
        return Ok();
    }

    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories()
    {
        return Ok();
    }
}