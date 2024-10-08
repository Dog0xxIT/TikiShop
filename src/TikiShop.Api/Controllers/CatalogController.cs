using MediatR;
using Microsoft.AspNetCore.Mvc;
using TikiShop.Core.RequestModels;
using TikiShop.Core.RequestModels.Catalog;
using TikiShop.Core.Services.CatalogService.Queries;

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
        var result = await _catalogQueries.GetListProducts(req);
        return Ok(result);
    }

    [HttpGet("products/{id}")]
    public async Task<IActionResult> GetProductById([FromRoute] int id)
    {
        var result = await _catalogQueries.GetProductById(id);
        return Ok(result);
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
        var result = await _catalogQueries.GetListBrands(paginationReq);
        return Ok(result);
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
        var result = await _catalogQueries.GetCategoriesHierarchy();
        return Ok(result);
    }

    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories()
    {
        var result = await _catalogQueries.GetCategories();
        return Ok(result);
    }
}