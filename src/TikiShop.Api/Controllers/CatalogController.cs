using TikiShop.Core.Services.CatalogService.Commands;
using TikiShop.Model.DTO;
using TikiShop.Model.Enums;
using TikiShop.Model.RequestModels;
using TikiShop.Model.RequestModels.Catalog;

namespace TikiShop.Api.Controllers;

[ApiController]
[Route("/api/v1/catalog")]
public class CatalogController : Controller
{
    private readonly ICatalogQueries _catalogQueries;
    private readonly IMediator _mediator;

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
        var command = new CreateProductCommand(
            req.Sku, req.Name, req.Description,
            req.ShortDescription, req.Price, req.ThumbnailUrl,
            req.Quantity, req.CategoryId, req.BrandId);
        var result = await _mediator.Send(command);
        if (result.ResultCode == ResultCode.Success)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpPut("products/{id}")]
    public async Task<IActionResult> UpdateProduct(
        [FromRoute] int id,
        [FromBody] UpdateProductRequest req)
    {
        var command = new UpdateProductCommand(
            id, req.Sku, req.Name,
            req.Description, req.ShortDescription,
            req.Price, req.ThumbnailUrl,
            req.Quantity, req.CategoryId, req.BrandId);
        var result = await _mediator.Send(command);
        if (result.ResultCode == ResultCode.Success)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpDelete("products/{id}")]
    public async Task<IActionResult> DeleteProduct([FromRoute] int id)
    {
        var command = new DeleteProductCommand(id);
        var result = await _mediator.Send(command);
        if (result.ResultCode == ResultCode.Success)
        {
            return Ok(result);
        }
        return BadRequest(result);
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
        var command = new CreateBrandCommand(req.Name);
        var result = await _mediator.Send(command);
        if (result.ResultCode == ResultCode.Success)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpPut("brands/{id}")]
    public async Task<IActionResult> UpdateBrand(
        [FromRoute] int id,
        [FromBody] UpdateBrandRequest req)
    {
        var command = new UpdateBrandCommand(id, req.Name);
        var result = await _mediator.Send(command);
        if (result.ResultCode == ResultCode.Success)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpDelete("brands/{id}")]
    public async Task<IActionResult> DeleteBrand([FromRoute] int id)
    {
        var command = new DeleteBrandCommand(id);
        var result = await _mediator.Send(command);
        if (result.ResultCode == ResultCode.Success)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpGet("categories/hierarchy")]
    public async Task<IActionResult> GetCategoriesHierarchy()
    {
        var result = await _catalogQueries.GetCategoriesHierarchy();
        return Ok(result);
    }

    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories([FromQuery] PaginationRequest paginationReq)
    {
        var result = await _catalogQueries.GetCategories(paginationReq);
        return Ok(result);
    }
}