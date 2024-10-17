using Microsoft.AspNetCore.Mvc;
using TikiShop.Core.Services.VnPayService;
using TikiShop.Infrastructure.Common;
using TikiShop.Shared.RequestModels.Payment;

namespace TikiShop.Api.Controllers
{
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
        public IActionResult GetVnPayment([FromRoute] int orderId)
        {
            var ipAddress = Utils.GetIpAddress(this.HttpContext);
            var result = _vnPayService.CreatePaymentUrl(orderId, ipAddress);
            if (!result.Succeeded)
            {
                return Problem("Error");
            }
            var paymentRequestUrl = result.Data ?? "";
            return Redirect(paymentRequestUrl);
        }

        [ProducesResponseType(StatusCodes.Status302Found)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpGet]
        [Route("vnpay-return")]
        public IActionResult VnPayReturnUrlCallback([FromQuery] ReturnUrlRequest returnUrlParams)
        {
            var result = _vnPayService.ReturnUrlVnPay(returnUrlParams);
            if (!result.Succeeded)
            {
                return Problem("Error");
            }
            var paymentRequestUrl = result.Data ?? "";
            return Redirect(paymentRequestUrl);
        }

        [ProducesResponseType(StatusCodes.Status302Found)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpGet]
        [Route("IPN")]
        public IActionResult VnPayIpnUrlCallback([FromQuery] ReturnUrlRequest returnUrlParams)
        {
            var result = _vnPayService.ReturnUrlVnPay(returnUrlParams);
            if (!result.Succeeded)
            {
                return Problem("Error");
            }
            var paymentRequestUrl = result.Data ?? "";
            return Redirect(paymentRequestUrl);
        }
    }
}
