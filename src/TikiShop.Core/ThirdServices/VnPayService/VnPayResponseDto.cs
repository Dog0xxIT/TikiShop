using TikiShop.Core.Utils;

namespace TikiShop.Core.ThirdServices.VnPayService;

public class VnPayResponseDto
{
    private VnPayResponseDto()
    {
    }

    public VnPayResponseDto(VnPayResponseCode code)
    {
        RspCode = code.ToString();
        RspCode = code.DescriptionAttr();
    }

    public string RspCode { get; private set; }
    public string Message { get; private set; }
}