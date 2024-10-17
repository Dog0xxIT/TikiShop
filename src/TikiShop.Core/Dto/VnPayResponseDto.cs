using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TikiShop.Core.Enums;
using TikiShop.Core.Utils;

namespace TikiShop.Core.Dto
{
    public class VnPayResponseDto
    {
        public string RspCode { get; private set; }
        public string Message { get; private set; }

        private VnPayResponseDto() { }

        public VnPayResponseDto(VnPayResponseCode code)
        {
            RspCode = code.ToString();
            RspCode = code.DescriptionAttr();
        }
    }
}
