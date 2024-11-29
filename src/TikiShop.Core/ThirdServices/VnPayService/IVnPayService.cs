using TikiShop.Model.DTO;
using TikiShop.Model.RequestModels.Payment;

namespace TikiShop.Core.ThirdServices.VnPayService;

public interface IVnPayService
{
    Task<ResultObject<string>> CreatePaymentUrl(int orderId, string clientIp);
    Task<ResultObject<VnPayResponseDto>> ReturnUrlVnPay(ReturnUrlReq returnUrlParams);
}