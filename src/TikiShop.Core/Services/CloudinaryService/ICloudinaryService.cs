﻿namespace TikiShop.Core.Services.CloudinaryService
{
    public interface ICloudinaryService
    {
        Task<Uri> UploadImage(string fileName, Stream steam);
        Task<ServiceResult> DeleteImage(int productId);
    }
}