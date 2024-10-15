using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TikiShop.Shared.RequestModels.Payment
{
    public class GetVnPaymentRequest
    {
        [Range(1, 8)]
        public string vnp_Version { get; set; }

        [Range(1, 16)]
        public string vnp_Command { get; set; }

        [Range(1, 16)]
        public string vnp_TmnCode { get; set; }

        [Range(1, 12)]
        public int vnp_Amount { get; set; }

        [Range(3, 20)]
        public string vnp_BankCode { get; set; }

        [Range(14, 14)]
        public int vnp_CreateDate { get; set; }

        [Range(3, 3)]
        public string vnp_CurrCode { get; set; }

        [Range(7, 45)]
        public string vnp_IpAddr { get; set; }

        [Range(2, 5)]
        public string vnp_Locale { get; set; }

        [Range(1, 255)]
        public string vnp_OrderInfo { get; set; }

        [Range(1, 100)]
        public int vnp_OrderType { get; set; }

        [Range(10, 255)]
        public string vnp_ReturnUrl { get; set; }

        [Range(14, 14)]
        public int vnp_ExpireDate { get; set; }

        [Range(1, 100)]
        public string vnp_TxnRef { get; set; }

        [Range(32, 256)]
        public string vnp_SecureHash { get; set; }
    }
}
