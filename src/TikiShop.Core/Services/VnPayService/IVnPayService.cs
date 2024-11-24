using TikiShop.Model.DTO;
using TikiShop.Model.RequestModels.Payment;

namespace TikiShop.Core.Services.VnPayService;

public interface IVnPayService
{
    Task<ServiceResult<string>> CreatePaymentUrl(int orderId, string clientIp);
    Task<ServiceResult<VnPayResponseDto>> ReturnUrlVnPay(ReturnUrlRequest returnUrlParams);
}