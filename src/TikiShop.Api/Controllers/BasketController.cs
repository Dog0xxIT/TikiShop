using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TikiShop.Core.Models.RequestModels.Basket;
using TikiShop.Core.Models.RequestModels.Catalog;
using TikiShop.Core.Services.BasketService.Commands;
using TikiShop.Core.Services.BasketService.Queries;
using TikiShop.Infrastructure.Common;

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

        [HttpPost("")]
        public async Task<IActionResult> CreateBasket()
        {
            var userId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.Sid));
            var command = new CreateBasketCommand(userId);
            var result = await _mediator.Send(command);
            if (!result.Succeeded)
            {
                return Problem(result.Errors.First());
            }

            return Ok();
        }

        [HttpGet("")]
        public async Task<IActionResult> GetBasketByCustomerId()
        {
            var userId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.Sid));
            var result = await _basketQueries.GetBasketByCustomerId(userId);
            return Ok(result);
        }

        [HttpPatch("addItem")]
        public async Task<IActionResult> AddToBasket(AddToBasketRequest req)
        {
            var userId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.Sid));
            var command = new AddToBasketCommand(userId, req.ProductId);
            var result = await _mediator.Send(command);
            if (!result.Succeeded)
            {
                return Problem(result.Errors.First());
            }

            return Ok();
        }

        [HttpPatch("updateQty")]
        public async Task<IActionResult> UpdateQty(UpdateQtyRequest req)
        {
            var userId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.Sid));
            var command = new UpdateQuantityCommand(userId, req.ProductId, req.Quantity);
            var result = await _mediator.Send(command);
            if (!result.Succeeded)
            {
                return Problem(result.Errors.First());
            }

            return Ok();
        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> Delete([FromRoute] int productId)
        {
            var userId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.Sid));
            var command = new DeleteBasketItemCommand(userId, productId);
            var result = await _mediator.Send(command);
            if (!result.Succeeded)
            {
                return Problem(result.Errors.First());
            }

            return Ok();
        }
    }
}