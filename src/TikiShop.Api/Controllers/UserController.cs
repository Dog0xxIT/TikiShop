using TikiShop.Core.Services.UserService.Commands;
using TikiShop.Core.Services.UserService.Queries;
using TikiShop.Infrastructure.Common;
using TikiShop.Shared.RequestModels.User;

namespace TikiShop.Api.Controllers
{
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

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("addresses")]
        public async Task<IActionResult> GetAllAddress()
        {
            var userId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.Sid));
            var result = await _userQueries.GetAllAddress(userId);
            return Ok(result);
        }

        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("addresses")]
        public async Task<IActionResult> CreateAddress(CreateAddressRequest req)
        {
            var userId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.Sid));
            var command = new CreateAddressCommand(req.Receiver, req.PhoneNumber, req.Street, req.City, req.State, req.Country, req.ZipCode, userId);
            var result = await _mediator.Send(command);
            if (!result.Succeeded)
            {
                return Problem(result.Errors.FirstOrDefault());
            }

            return Created();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPut("addresses/{id}")]
        public async Task<IActionResult> UpdateAddress([FromRoute] int id, UpdateAddressRequest req)
        {
            var command = new UpdateAddressCommand(id, req.Receiver, req.PhoneNumber, req.Street, req.City, req.State, req.Country, req.ZipCode);
            var result = await _mediator.Send(command);
            if (!result.Succeeded)
            {
                return Problem(result.Errors.FirstOrDefault());
            }

            return Created();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpDelete("addresses/{id}")]
        public async Task<IActionResult> DeleteAddress([FromRoute] int id)
        {
            var command = new DeleteAddressCommand(id);
            var result = await _mediator.Send(command);
            if (!result.Succeeded)
            {
                return Problem(result.Errors.FirstOrDefault());
            }

            return Created();
        }

        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("payments")]
        public async Task<IActionResult> CreatePayment()
        {
            return Created();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPatch("payments/{id}")]
        public async Task<IActionResult> UpdatePayment([FromRoute] int id)
        {
            return Ok();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpDelete("payments/{id}")]
        public async Task<IActionResult> DeletePayment([FromRoute] int id)
        {
            return Ok();
        }
    }
}
