using TikiShop.Model.DTO;

namespace TikiShop.Core.ThirdServices.CloudinaryService;

public interface ICloudinaryService
{
    Task<Uri> UploadImage(string fileName, Stream steam);
    Task<ResultObject<int>> DeleteImage(int productId);
}