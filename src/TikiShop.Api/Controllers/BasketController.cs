using TikiShop.Core.Services.BasketService.Commands;
using TikiShop.Infrastructure.Common;
using TikiShop.Shared.RequestModels.Basket;

namespace TikiShop.Api.Controllers
{
    [Authorize(Roles = RolesConstant.Customer)]
    [ApiController]
    [Route("api/v1/baskets")]
    public class BasketController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IBasketQueries _basketQueries;

        public BasketController(IBasketQueries basketQueries, IMediator mediator)
        {
            _basketQueries = basketQueries;
            _mediator = mediator;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpGet]
        public async Task<IActionResult> GetBasketByCustomerId()
        {
            var userId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.Sid));
            var result = await _basketQueries.GetBasketByCustomerId(userId);
            return Ok(result);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPatch]
        public async Task<IActionResult> UpdateBasketItem(UpdateBasketItemRequest req)
        {
            var userId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.Sid));
            var command = new UpdateQuantityCommand(userId, req.ProductSkuId, req.Quantity);
            var result = await _mediator.Send(command);
            if (!result.Succeeded)
            {
                return Problem(result.Errors.FirstOrDefault());
            }

            return Ok();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpDelete("{basketItemId}")]
        public async Task<IActionResult> DeleteBasketItem([FromRoute] int basketItemId)
        {
            var command = new DeleteBasketItemCommand(basketItemId);
            var result = await _mediator.Send(command);
            if (!result.Succeeded)
            {
                return Problem(result.Errors.FirstOrDefault());
            }

            return Ok();
        }
    }
}