using MediatR;
using Microsoft.AspNetCore.Mvc;
using TikiShop.Core.Services.OrderService.Queries;

namespace TikiShop.Api.Controllers
{
    [ApiController]
    [Route("/api/v1/orders")]
    public class OrderController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IOrderQueries _orderQueries;

        public OrderController(IOrderQueries orderQueries, IMediator mediator)
        {
            _orderQueries = orderQueries;
            _mediator = mediator;
        }

        [HttpGet("user/{buyerId}")]
        public async Task<IActionResult> GetOrderByBuyerId([FromRoute] int buyerId)
        {
            var result = await _orderQueries.GetOrderByBuyerId(buyerId);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById([FromRoute] int id)
        {
            var result = await _orderQueries.GetOrderByBuyerId(id);
            return Ok(result);
        }

        [HttpGet("/cartTypes")]
        public async Task<IActionResult> CartTypes()
        {
            var result = await _orderQueries.CartTypes();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create()
        {
            return Ok();
        }

        [HttpPost("draft")]
        public async Task<IActionResult> CreateDraft()
        {
            //var orderDraft = new Order(req.BuyerId);
            return Ok();
        }


        [HttpPatch("cancel/{id}")]
        public async Task<IActionResult> Cancel([FromRoute] int id)
        {
            return Ok();
        }

        [HttpPatch("ship/{id}")]
        public async Task<IActionResult> Ship([FromRoute] int id)
        {
            return Ok();
        }
    }
}
