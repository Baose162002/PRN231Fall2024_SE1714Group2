using CloudinaryDotNet.Actions;
using CloudinaryDotNet;

namespace PRN231_2_EventFlowerExchange_FE.Service
{
    public class CloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IConfiguration configuration)
        {
            var cloudinaryConfig = new Account(
                configuration["CloudinarySettings:CloudName"],
                configuration["CloudinarySettings:ApiKey"],
                configuration["CloudinarySettings:ApiSecret"]);

            _cloudinary = new Cloudinary(cloudinaryConfig);
        }

        public async Task<string> UploadImageAsync(IFormFile imageFile)
        {
            using var stream = new MemoryStream();
            await imageFile.CopyToAsync(stream);

            // Reset the stream position to the beginning before sending it to Cloudinary
            stream.Position = 0;

            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(imageFile.FileName, stream),
                PublicId = $"flower_imgs/{Path.GetFileNameWithoutExtension(imageFile.FileName)}"
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception($"Image upload failed: {uploadResult.Error?.Message}");
            }

            return uploadResult.SecureUri?.ToString();
        }
    }
}
