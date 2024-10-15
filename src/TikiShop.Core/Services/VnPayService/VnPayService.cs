using Microsoft.Extensions.Options;
using TikiShop.Core.Configurations;
using TikiShop.Core.Utils;
using TikiShop.Shared.RequestModels.Payment;

namespace TikiShop.Core.Services.VnPayService;

public class VnPayService : IVnPayService
{
    private readonly ILogger<VnPayService> _logger;
    private readonly VnPayConfig _vnPayConfig;

    public VnPayService(ILogger<VnPayService> logger, IOptions<VnPayConfig> vnPayConfig)
    {
        _logger = logger;
        _vnPayConfig = vnPayConfig.Value;
    }

    public ServiceResult<string> CreatePaymentUrl()
    {
        var req = new GetVnPaymentRequest
        {
            vnp_Version = "2.1.0",
            vnp_Amount = 1,
            vnp_BankCode = "",
            vnp_Command = "pay",
            vnp_CreateDate = Helper.ConvertToDateInt(DateTime.UtcNow),
            vnp_CurrCode = "",
            vnp_ExpireDate = Helper.ConvertToDateInt(DateTime.UtcNow.AddYears(2)),
            vnp_IpAddr = "192.168.0.1",
            vnp_Locale = "vn",
            vnp_OrderInfo = "Nap tien cho thue bao 0123456789. So tien 100,000 VND",
            vnp_OrderType = 1,
            vnp_ReturnUrl = _vnPayConfig.vnp_Returnurl,
            vnp_SecureHash = _vnPayConfig.vnp_HashSecret,
            vnp_TmnCode = _vnPayConfig.vnp_TmnCode,
            vnp_TxnRef = ""
        };
        return ServiceResult<string>.Success("");
    }

    public ServiceResult<string> ReturnUrlVnPay(ReturnUrlVnPayRequest req)
    {
        return ServiceResult<string>.Success("");
    }
}