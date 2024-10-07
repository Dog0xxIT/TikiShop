using MediatR;
using Microsoft.AspNetCore.Mvc;
using TikiShop.Core.Services.OrderService.Queries;
using TikiShop.Share.ResponseModels;

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
        public async Task<IActionResult> GetOrderByBuyerId([FromRoute] string buyerId)
        {
            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById([FromRoute] string id)
        {
            return Ok();
        }

        [HttpGet("/cartTypes")]
        public async Task<IActionResult> CartTypes()
        {
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Create()
        {
            return Ok(ResponseObject.Succeeded);
        }

        [HttpPost("draft")]
        public async Task<IActionResult> CreateDraft()
        {
            //var orderDraft = new Order(req.BuyerId);
            return Ok(ResponseObject.Succeeded);
        }


        [HttpPatch("cancel/{id}")]
        public async Task<IActionResult> Cancel([FromRoute] int id)
        {
            return Ok(ResponseObject.Succeeded);
        }

        [HttpPatch("ship/{id}")]
        public async Task<IActionResult> Ship([FromRoute] int id)
        {
            return Ok(ResponseObject.Succeeded);
        }
    }
}
