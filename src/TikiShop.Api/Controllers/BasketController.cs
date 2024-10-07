using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TikiShop.Api.Configurations;
using TikiShop.Core.Common;
using TikiShop.Core.Services.BasketService.Queries;
using TikiShop.Share.RequestModels.Basket;

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
            return Ok();
        }

        [HttpGet("{buyerId}")]
        public async Task<IActionResult> GetBasketByCustomerId(int buyerId)
        {
            return Ok();
        }

        [HttpPatch("/addToBakset")]
        public async Task<IActionResult> AddToBasket(AddToBasketRequest req)
        {
            return Ok();
        }

        [HttpPatch("updateQty")]
        public async Task<IActionResult> UpdateQty(UpdateQtyRequest req)
        {
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            return Ok();
        }
    }
}