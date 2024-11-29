using TikiShop.Core.ThirdServices.VnPayService;
using TikiShop.Model.Enums;
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

    [Authorize(Roles = RolesConstant.Customer)]
    [HttpGet]
    [Route("vnpay/{orderId}")]
    public async Task<IActionResult> GetVnPayment([FromRoute] int orderId)
    {
        var ipAddress = Utils.GetIpAddress(HttpContext);
        var result = await _vnPayService.CreatePaymentUrl(orderId, ipAddress);
        if (result.ResultCode == ResultCode.Failure) return Problem("Error");
        var paymentRequestUrl = result.Data ?? "";
        return Redirect(paymentRequestUrl);
    }

    [HttpGet]
    [Route("vnpay-return")]
    public async Task<IActionResult> VnPayReturnUrlCallback([FromQuery] ReturnUrlReq returnUrlParams)
    {
        var result = await _vnPayService.ReturnUrlVnPay(returnUrlParams);
        if (result.ResultCode == ResultCode.Failure) return Problem("Error");
        var paymentRequestUrl = result.Data;
        return Redirect("");
    }

    [HttpGet]
    [Route("IPN")]
    public async Task<IActionResult> VnPayIpnUrlCallback([FromQuery] ReturnUrlReq returnUrlParams)
    {
        var result = await _vnPayService.ReturnUrlVnPay(returnUrlParams);
        if (result.ResultCode == ResultCode.Failure) return Problem("Error");

        var paymentRequestUrl = result.Data;
        return Redirect("paymentRequestUrl");
    }
}