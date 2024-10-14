using Microsoft.AspNetCore.Mvc;

namespace TikiShop.Api.Controllers
{
    public class PaymentController : Controller
    {
        public IActionResult GetVnPayment()
        {
            return Ok();
        }
    }
}
