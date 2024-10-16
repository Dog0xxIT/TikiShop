using Microsoft.AspNetCore.Mvc;
using TikiShop.Core.Services.VnPayService;

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
        [HttpGet]
        [Route("vnpay")]
        public IActionResult GetVnPayment()
        {
            var result = _vnPayService.CreatePaymentUrl();
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
        public IActionResult VnPayReturnUrlCallback()
        {
            var result = _vnPayService.ReturnUrlVnPay(new());
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
        public IActionResult VnPayIpnUrlCallback()
        {
            var result = _vnPayService.ReturnUrlVnPay(new());
            if (!result.Succeeded)
            {
                return Problem("Error");
            }
            var paymentRequestUrl = result.Data ?? "";
            return Redirect(paymentRequestUrl);
        }
    }
}
