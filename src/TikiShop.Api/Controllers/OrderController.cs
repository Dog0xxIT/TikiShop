namespace TikiShop.Api.Controllers;

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

    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpGet("user/{buyerId}")]
    public async Task<IActionResult> GetOrderByBuyerId([FromRoute] int buyerId)
    {
        var result = await _orderQueries.GetOrderByBuyerId(buyerId);
        return Ok(result);
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderById([FromRoute] int id)
    {
        var result = await _orderQueries.GetOrderByBuyerId(id);
        return Ok(result);
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpGet("/cartTypes")]
    public async Task<IActionResult> CartTypes()
    {
        var result = await _orderQueries.CartTypes();
        return Ok(result);
    }

    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPost]
    public async Task<IActionResult> Create()
    {
        return Created();
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPost("draft")]
    public async Task<IActionResult> CreateDraft()
    {
        //var orderDraft = new Order(req.BuyerId);
        return Created();
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPatch("cancel/{id}")]
    public async Task<IActionResult> Cancel([FromRoute] int id)
    {
        return Ok();
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPatch("ship/{id}")]
    public async Task<IActionResult> Ship([FromRoute] int id)
    {
        return Ok();
    }
}