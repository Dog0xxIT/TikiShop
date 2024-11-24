using TikiShop.Model.RequestModels.Payment;

namespace TikiShop.Api.Controllers;

[ApiController]
[Route("api/v1/payments")]
public class PaymentController : Controller
{
    private readonly IVnPayService _vnPayService;

    public PaymentController(IVnPayService vnPayService)
    {
        _vnPayService = vnPayService;
    }

    [ProducesResponseType(StatusCodes.Status302Found)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [Authorize(Roles = RolesConstant.Customer)]
    [HttpGet]
    [Route("vnpay/{orderId}")]
    public async Task<IActionResult> GetVnPayment([FromRoute] int orderId)
    {
        var ipAddress = Utils.GetIpAddress(HttpContext);
        var result = await _vnPayService.CreatePaymentUrl(orderId, ipAddress);
        if (!result.Succeeded) return Problem("Error");
        var paymentRequestUrl = result.Data ?? "";
        return Redirect(paymentRequestUrl);
    }

    [ProducesResponseType(StatusCodes.Status302Found)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [HttpGet]
    [Route("vnpay-return")]
    public async Task<IActionResult> VnPayReturnUrlCallback([FromQuery] ReturnUrlRequest returnUrlParams)
    {
        var result = await _vnPayService.ReturnUrlVnPay(returnUrlParams);
        if (!result.Succeeded) return Problem("Error");
        var paymentRequestUrl = result.Data;
        return Redirect("");
    }

    [ProducesResponseType(StatusCodes.Status302Found)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [HttpGet]
    [Route("IPN")]
    public async Task<IActionResult> VnPayIpnUrlCallback([FromQuery] ReturnUrlRequest returnUrlParams)
    {
        var result = await _vnPayService.ReturnUrlVnPay(returnUrlParams);
        if (!result.Succeeded) return Problem("Error");

        var paymentRequestUrl = result.Data;
        return Redirect("paymentRequestUrl");
    }
}