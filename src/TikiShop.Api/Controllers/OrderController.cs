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

    /// <summary>
    /// Retrieves the order details for a given buyer ID.
    /// </summary>
    /// <param name="buyerId">The ID of the buyer to fetch the orders for.</param>
    /// <returns>An HTTP response with the order details for the specified buyer.</returns>
    [HttpGet("user/{buyerId}")]
    public async Task<IActionResult> GetOrderByBuyerId([FromRoute] int buyerId)
    {
        var result = await _orderQueries.GetOrderByBuyerId(buyerId);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves the order details for a specific order ID.
    /// </summary>
    /// <param name="id">The ID of the order to fetch.</param>
    /// <returns>An HTTP response with the order details for the specified order.</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderById([FromRoute] int id)
    {
        var result = await _orderQueries.GetOrderByBuyerId(id);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves the list of available cart types.
    /// </summary>
    /// <returns>An HTTP response with the list of cart types.</returns>
    [HttpGet("/cartTypes")]
    public async Task<IActionResult> CartTypes()
    {
        var result = await _orderQueries.CartTypes();
        return Ok(result);
    }

    /// <summary>
    /// Creates a new order.
    /// </summary>
    /// <returns>An HTTP response indicating the creation of the order.</returns>
    [HttpPost]
    public async Task<IActionResult> Create()
    {
        return Created();
    }

    /// <summary>
    /// Creates a draft order.
    /// </summary>
    /// <returns>An HTTP response indicating the creation of a draft order.</returns>
    [HttpPost("draft")]
    public async Task<IActionResult> CreateDraft()
    {
        //var orderDraft = new Order(req.BuyerId);
        return Created();
    }

    /// <summary>
    /// Cancels an existing order by its ID.
    /// </summary>
    /// <param name="id">The ID of the order to cancel.</param>
    /// <returns>An HTTP response indicating whether the cancellation was successful.</returns>
    [HttpPatch("cancel/{id}")]
    public async Task<IActionResult> Cancel([FromRoute] int id)
    {
        return Ok();
    }

    /// <summary>
    /// Ships an order by its ID.
    /// </summary>
    /// <param name="id">The ID of the order to mark as shipped.</param>
    /// <returns>An HTTP response indicating whether the shipping was successful.</returns>
    [HttpPatch("ship/{id}")]
    public async Task<IActionResult> Ship([FromRoute] int id)
    {
        return Ok();
    }

}