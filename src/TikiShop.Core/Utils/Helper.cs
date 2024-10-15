using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TikiShop.Core.Utils
{
    internal static class Helper
    {
        internal static string HashHMACSHA512(string input, string secretKey)
        {
            var inputBytes = Encoding.UTF8.GetBytes(input);
            var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);
            var hmac512 = new HMACSHA512(secretKeyBytes);
            var hashBytes = hmac512.ComputeHash(inputBytes);
            var sb = new StringBuilder();
            foreach (var x in hashBytes)
            {
                sb.Append($"{x:x2}");
            }
            return sb.ToString(); // The resulting string is 64 characters long
        }

        /// <summary>
        ///    Convert To Date Int yyyyMMddHHmmss
        /// </summary>
        /// <param name="input"></param>
        /// <returns>int yyyyMMddHHmmss Format</returns>    
        internal static int ConvertToDateInt(DateTime dateTime)
        {
            return Convert.ToInt32(dateTime.ToString("yyyyMMddHHmmss"));
        }
    }
}
