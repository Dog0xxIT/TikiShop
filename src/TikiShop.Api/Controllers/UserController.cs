using TikiShop.Core.Services.UserService.Commands;
using TikiShop.Model.Enums;
using TikiShop.Model.RequestModels.User;

namespace TikiShop.Api.Controllers;

[Authorize(Roles = RolesConstant.Customer)]
[ApiController]
[Route("api/v1/")]
public class UserController : Controller
{
    private readonly IMediator _mediator;
    private readonly IUserQueries _userQueries;

    public UserController(IUserQueries userQueries, IMediator mediator)
    {
        _userQueries = userQueries;
        _mediator = mediator;
    }

    [HttpGet("addresses")]
    public async Task<IActionResult> GetAllAddress()
    {
        var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.Sid));
        var result = await _userQueries.GetAllAddress(userId);
        return Ok(result);
    }

    [HttpPost("addresses")]
    public async Task<IActionResult> CreateAddress(CreateAddressReq req)
    {
        var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.Sid));
        var command = new CreateAddressCommand(req.Receiver, req.PhoneNumber, req.Street, req.City, req.State,
            req.Country, req.ZipCode, userId);
        var result = await _mediator.Send(command);
        if (result.ResultCode == ResultCode.Success)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpPut("addresses/{id}")]
    public async Task<IActionResult> UpdateAddress([FromRoute] int id, UpdateAddressReq req)
    {
        var command = new UpdateAddressCommand(id, req.Receiver, req.PhoneNumber, req.Street, req.City, req.State,
            req.Country, req.ZipCode);
        var result = await _mediator.Send(command);
        if (result.ResultCode == ResultCode.Success)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpDelete("addresses/{id}")]
    public async Task<IActionResult> DeleteAddress([FromRoute] int id)
    {
        var command = new DeleteAddressCommand(id);
        var result = await _mediator.Send(command);
        if (result.ResultCode == ResultCode.Success)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpPost("payments")]
    public async Task<IActionResult> CreatePayment()
    {
        return Created();
    }

    [HttpPatch("payments/{id}")]
    public async Task<IActionResult> UpdatePayment([FromRoute] int id)
    {
        return Ok();
    }

    [HttpDelete("payments/{id}")]
    public async Task<IActionResult> DeletePayment([FromRoute] int id)
    {
        return Ok();
    }
}