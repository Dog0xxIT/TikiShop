using TikiShop.Core.Services.CatalogService.Commands;
using TikiShop.Model.DTO;
using TikiShop.Model.Enums;
using TikiShop.Model.RequestModels;
using TikiShop.Model.RequestModels.Catalog;
using TikiShop.Model.ResponseModels;
using TikiShop.Model.ResponseModels.Catalog;

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

    /// <summary>
    /// Retrieves a list of products based on the provided filtering criteria.
    /// </summary>
    /// <param name="req">The request object containing pagination and filtering parameters.</param>
    /// <returns>A list of products that match the criteria.</returns>
    [HttpGet("products")]
    public async Task<IActionResult> GetListProducts([FromQuery] GetListProductReq req)
    {
        var result = await _catalogQueries.GetListProducts(req);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves details of a product by its ID.
    /// </summary>
    /// <param name="id">The unique identifier of the product.</param>
    /// <returns>The product details if found; otherwise, a 404 response.</returns>
    [HttpGet("products/{id}")]
    public async Task<IActionResult> GetProductById([FromRoute] int id)
    {
        var result = await _catalogQueries.GetProductById(id);
        return Ok(result);
    }

    /// <summary>
    /// Creates a new product in the catalog.
    /// </summary>
    /// <param name="req">The request object containing product details to create.</param>
    /// <returns>A success response if the product is created; otherwise, a bad request response.</returns>
    [HttpPost("products")]
    public async Task<IActionResult> CreateProduct(CreateProductReq req)
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

    /// <summary>
    /// Updates the details of an existing product by its ID.
    /// </summary>
    /// <param name="id">The unique identifier of the product to update.</param>
    /// <param name="req">The request object containing updated product details.</param>
    /// <returns>A success response if the update is successful; otherwise, a bad request response.</returns>
    [HttpPut("products/{id}")]
    public async Task<IActionResult> UpdateProduct(
        [FromRoute] int id,
        [FromBody] UpdateProductReq req)
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

    /// <summary>
    /// Deletes a product from the catalog by its ID.
    /// </summary>
    /// <param name="id">The unique identifier of the product to delete.</param>
    /// <returns>A success response if the deletion is successful; otherwise, a bad request response.</returns>
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

    /// <summary>
    /// Retrieves a paginated list of brands.
    /// </summary>
    /// <param name="paginationReq">The request object containing pagination parameters.</param>
    /// <returns>A list of brands.</returns>
    [HttpGet("brands")]
    public async Task<IActionResult> GetListBrands([FromQuery] PaginationReq paginationReq)
    {
        var result = await _catalogQueries.GetListBrands(paginationReq);
        return Ok(result);
    }

    /// <summary>
    /// Creates a new brand in the catalog.
    /// </summary>
    /// <param name="req">The request object containing brand details.</param>
    /// <returns>A success response if the brand is created; otherwise, a bad request response.</returns>
    [HttpPost("brands")]
    public async Task<IActionResult> CreateBrand(CreateBrandReq req)
    {
        var command = new CreateBrandCommand(req.Name);
        var result = await _mediator.Send(command);
        if (result.ResultCode == ResultCode.Success)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    /// <summary>
    /// Updates the details of a brand by its ID.
    /// </summary>
    /// <param name="id">The unique identifier of the brand to update.</param>
    /// <param name="req">The request object containing the updated brand details.</param>
    /// <returns>
    /// A success response with the updated brand details if the operation is successful; 
    /// otherwise, a bad request response with the error details.
    /// </returns>
    [HttpPut("brands/{id}")]
    public async Task<IActionResult> UpdateBrand(
        [FromRoute] int id,
        [FromBody] UpdateBrandReq req)
    {
        var command = new UpdateBrandCommand(id, req.Name);
        var result = await _mediator.Send(command);
        if (result.ResultCode == ResultCode.Success)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    /// <summary>
    /// Deletes a brand by its ID.
    /// </summary>
    /// <param name="id">The unique identifier of the brand to delete.</param>
    /// <returns>
    /// A success response if the brand is deleted successfully; 
    /// otherwise, a bad request response with the error details.
    /// </returns>
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


    /// <summary>
    /// Retrieves the hierarchical structure of categories.
    /// </summary>
    /// <returns>A tree structure of categories.</returns>
    [HttpGet("categories/hierarchy")]
    public async Task<IActionResult> GetCategoriesHierarchy()
    {
        var result = await _catalogQueries.GetCategoriesHierarchy();
        return Ok(result);
    }

    /// <summary>
    /// Retrieves a paginated list of categories.
    /// </summary>
    /// <param name="paginationReq">The request object containing pagination parameters.</param>
    /// <returns>A paginated list of categories.</returns>
    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories([FromQuery] PaginationReq paginationReq)
    {
        var result = await _catalogQueries.GetCategories(paginationReq);
        return Ok(result);
    }
}