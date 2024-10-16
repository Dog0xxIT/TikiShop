using Azure;
using Microsoft.Extensions.Options;
using System.Web;
using TikiShop.Core.Configurations;
using TikiShop.Core.Utils;
using TikiShop.Infrastructure.Models;
using TikiShop.Shared.RequestModels.Payment;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
            vnp_Amount = "1000000",
            vnp_BankCode = "",
            vnp_Command = "pay",
            vnp_CreateDate = Helper.ConvertToDateIntString(DateTime.UtcNow.AddHours(7)),
            vnp_CurrCode = "VND",
            vnp_ExpireDate = Helper.ConvertToDateIntString(DateTime.UtcNow.AddHours(7).AddMinutes(15)),
            vnp_IpAddr = "1.53.52.219",
            vnp_Locale = "vn",
            vnp_OrderInfo = "Nap tien cho thue bao 0123456789. So tien 100,000 VND",
            vnp_OrderType = "100000",
            vnp_ReturnUrl = _vnPayConfig.vnp_Returnurl,
            //vnp_SecureHash = _vnPayConfig.vnp_HashSecret,
            vnp_TmnCode = _vnPayConfig.vnp_TmnCode,
            vnp_TxnRef = DateTime.Now.Ticks.ToString()
        };

        var vnPayLibrary = new VnPayLibrary();
        foreach (var property in req.GetType().GetProperties())
        {
            var value = property.GetValue(req) as string;
            vnPayLibrary.AddRequestData(property.Name, value);
        }
        var paymentUrl = vnPayLibrary.CreateRequestUrl(_vnPayConfig.vnp_Url, _vnPayConfig.vnp_HashSecret);
        return ServiceResult<string>.Success(paymentUrl);
    }

    public ServiceResult<string> ReturnUrlVnPay(ReturnUrlVnPayRequest req)
    {
        return ServiceResult<string>.Success("");
    }
}