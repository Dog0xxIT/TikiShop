using TikiShop.Core.Services.BasketService.Commands;
using TikiShop.Model.Enums;
using TikiShop.Model.RequestModels.Basket;

namespace TikiShop.Api.Controllers;

[Authorize(Roles = RolesConstant.Customer)]
[ApiController]
[Route("api/v1/baskets")]
public class BasketController : Controller
{
    private readonly IBasketQueries _basketQueries;
    private readonly IMediator _mediator;

    public BasketController(IBasketQueries basketQueries, IMediator mediator)
    {
        _basketQueries = basketQueries;
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves the basket of a customer based on their user ID.
    /// </summary>
    /// <returns>An HTTP response with the customer's basket details.</returns>
    [HttpGet]
    public async Task<IActionResult> GetBasketByCustomerId()
    {
        var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.Sid));
        var result = await _basketQueries.GetBasketByCustomerId(userId);
        return Ok(result);
    }

    /// <summary>
    /// Updates the quantity of an item in the user's basket.
    /// </summary>
    /// <param name="req">The request details containing the product ID and new quantity.</param>
    /// <returns>An HTTP response indicating success or failure of the update operation.</returns>
    [HttpPatch]
    public async Task<IActionResult> UpdateBasketItem(UpdateBasketItemReq req)
    {
        var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.Sid));
        var command = new UpdateQuantityCommand(userId, req.ProductId, req.Quantity);
        var result = await _mediator.Send(command);
        if (result.ResultCode == ResultCode.Success)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    /// <summary>
    /// Deletes an item from the user's basket.
    /// </summary>
    /// <param name="basketItemId">The ID of the basket item to delete.</param>
    /// <returns>An HTTP response indicating success or failure of the deletion operation.</returns>
    [HttpDelete("{basketItemId}")]
    public async Task<IActionResult> DeleteBasketItem([FromRoute] int basketItemId)
    {
        var command = new DeleteBasketItemCommand(basketItemId);
        var result = await _mediator.Send(command);
        if (result.ResultCode == ResultCode.Success)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

}