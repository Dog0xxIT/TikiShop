using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using dotenv.net;

namespace TikiShop.Core.Services.CloudinaryService
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;
        private readonly ILogger<CloudinaryService> _logger;

        public CloudinaryService(ILogger<CloudinaryService> logger)
        {
            _logger = logger;
            DotEnv.Load(options: new DotEnvOptions(probeForEnv: true));
            _cloudinary = new Cloudinary(Environment.GetEnvironmentVariable("CLOUDINARY_URL"));
            _cloudinary.Api.Secure = true;
        }

        public async Task<Uri> UploadImage(string fileName, Stream stream)
        {
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(fileName, stream),
                Folder = "EShop"
            };

            try
            {
                var result = await _cloudinary.UploadAsync(uploadParams);
                _logger.LogInformation($"Image uploaded: {result.JsonObj}");
                return result.SecureUrl;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while uploading the image.");
                throw; // Re-throw the exception for further handling if necessary
            }
        }

        public async Task<ServiceResult> DeleteImage(int productId)
        {
            throw new NotImplementedException();
        }
    }
}
