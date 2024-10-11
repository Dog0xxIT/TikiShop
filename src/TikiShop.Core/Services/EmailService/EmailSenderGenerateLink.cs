using Microsoft.AspNetCore.WebUtilities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace TikiShop.Core.Services.EmailService
{
    public static class EmailSenderGenerateLink
    {
        public static string GenerateConfirmLink(string code, string email)
        {
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var link = HtmlEncoder.Default.Encode($"https://localhost:7014/api/v1/confirmEmail?Code={code}&Email={email}");
            return link;
        }

        public static string GenerateResetLink(string code, string email)
        {
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var link = HtmlEncoder.Default.Encode($"https://localhost:7014/api/v1/confirmEmail?Code={code}&Email={email}");
            return link;
        }
    }
}
