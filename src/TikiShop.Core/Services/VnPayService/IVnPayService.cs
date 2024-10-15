using TikiShop.Shared.RequestModels.Payment;

namespace TikiShop.Core.Services.VnPayService;

public interface IVnPayService
{
    ServiceResult<string> CreatePaymentUrl();
    ServiceResult<string> ReturnUrlVnPay(ReturnUrlVnPayRequest req);
}