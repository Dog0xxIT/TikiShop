using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TikiShop.Core.Configurations
{
    public class VnPayConfig
    {
        public static readonly string SectionName = "VnPayConfig";
        public string vnp_TmnCode { get; set; }
        public string vnp_HashSecret { get; set; }
        public string vnp_Returnurl { get; set; }
        public string vnp_Url { get; set; }
    }
}