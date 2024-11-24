using Microsoft.Extensions.Options;
using TikiShop.Core.Utils;
using TikiShop.Model.Configurations;
using TikiShop.Model.DTO;
using TikiShop.Model.Enums;
using TikiShop.Model.RequestModels.Payment;
using OrderStatus = TikiShop.Infrastructure.Common.OrderStatus;

namespace TikiShop.Core.Services.VnPayService;

public class VnPayService : IVnPayService
{
    private readonly TikiShopDbContext _context;
    private readonly ILogger<VnPayService> _logger;
    private readonly VnPayConfig _vnPayConfig;

    public VnPayService(ILogger<VnPayService> logger, IOptions<VnPayConfig> vnPayConfig, TikiShopDbContext context)
    {
        _logger = logger;
        _context = context;
        _vnPayConfig = vnPayConfig.Value;
    }

    public async Task<ServiceResult<string>> CreatePaymentUrl(int orderId, string clientIp)
    {
        var order = await _context.Orders
            .AsNoTracking()
            .Select(o => new
            {
                o.Id,
                Amount = o.Total,
                Status = o.OrderStatus,
                o.Description
            })
            .SingleOrDefaultAsync(o => o.Id == orderId);

        if (order is null || order.Status != OrderStatus.Submitted)
        {
            _logger.LogWarning($"Invalid order with ID: {orderId}"); // Added logging
            return ServiceResult<string>.Failed("Invalid order");
        }

        var req = new CreateVnPayUrlDto
        {
            vnp_Version = "2.1.0",
            vnp_Amount = Convert.ToInt32(order.Amount).ToString(),
            vnp_BankCode = "",
            vnp_Command = "pay",
            vnp_CreateDate = Helper.ConvertToDateIntString(DateTime.UtcNow.AddHours(7)),
            vnp_CurrCode = "VND",
            vnp_ExpireDate = Helper.ConvertToDateIntString(DateTime.UtcNow.AddHours(7).AddMinutes(15)),
            vnp_IpAddr = clientIp,
            vnp_Locale = "vn",
            vnp_OrderInfo = Helper.RemoveUnicode(order.Description),
            vnp_OrderType = "100000",
            vnp_ReturnUrl = _vnPayConfig.vnp_Returnurl,
            vnp_TmnCode = _vnPayConfig.vnp_TmnCode,
            vnp_TxnRef = order.Id.ToString()
        };

        var vnPayLibrary = new VnPayLibrary();
        foreach (var property in req.GetType().GetProperties())
        {
            var value = property.GetValue(req) as string;
            vnPayLibrary.AddRequestData(property.Name, value);
        }

        var paymentUrl = vnPayLibrary.CreateRequestUrl(_vnPayConfig.vnp_Url, _vnPayConfig.vnp_HashSecret);
        _logger.LogInformation($"Payment URL created for Order ID: {orderId}, URL: {paymentUrl}"); // Added logging
        return ServiceResult<string>.Success(paymentUrl);
    }

    public async Task<ServiceResult<VnPayResponseDto>> ReturnUrlVnPay(ReturnUrlRequest returnUrlParams)
    {
        var vnpay = new VnPayLibrary();
        var isValid = vnpay.ValidateSignature(returnUrlParams.vnp_SecureHash, _vnPayConfig.vnp_HashSecret);
        if (!isValid)
        {
            _logger.LogWarning($"Invalid signature, InputData={returnUrlParams}"); // Added logging
            return ServiceResult<VnPayResponseDto>.Success(new VnPayResponseDto(VnPayResponseCode.InvalidSignature));
        }

        var order = await _context.Orders.FindAsync(returnUrlParams.vnp_TxnRef);
        Enum.TryParse<VnPayResponseCode>(returnUrlParams.vnp_ResponseCode.ToString(), out var responseCode);
        Enum.TryParse<VnPayResponseCode>(returnUrlParams.vnp_TransactionStatus.ToString(), out var transactionStatus);

        if (order is null)
        {
            _logger.LogWarning($"Order not found for TxnRef: {returnUrlParams.vnp_TxnRef}"); // Added logging
            return ServiceResult<VnPayResponseDto>.Success(new VnPayResponseDto(VnPayResponseCode.OtherErrors));
        }

        if (Convert.ToInt32(order.Total) == returnUrlParams.vnp_Amount)
            return ServiceResult<VnPayResponseDto>.Success(new VnPayResponseDto(VnPayResponseCode.PartialRefundOnly));

        if (order.OrderStatus == OrderStatus.AwaitingValidation)
            return ServiceResult<VnPayResponseDto>.Success(new VnPayResponseDto(VnPayResponseCode.TransactionNotFound));

        if (order.OrderStatus == OrderStatus.Submitted &&
            responseCode == VnPayResponseCode.Success &&
            transactionStatus == VnPayResponseCode.Success)
        {
            // Thanh toan thanh cong
            _logger.LogInformation($"Payment successful  for Order ID: {order.Id}"); // Added logging
            order.OrderStatus = OrderStatus.Paid;
            return ServiceResult<VnPayResponseDto>.Success(new VnPayResponseDto(VnPayResponseCode.Success));
        }

        _logger.LogInformation($"Payment failed for Order ID: {order.Id}"); // Added logging
        return ServiceResult<VnPayResponseDto>.Success(new VnPayResponseDto(VnPayResponseCode.OtherErrors));
    }
}