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

        public async Task<string> UploadImageAsync1(string base64Image)
        {
            // Verify the base64 string is valid
            if (string.IsNullOrEmpty(base64Image))
            {
                throw new ArgumentException("Base64 image string is null or empty.");
            }

            // Check if the base64 string contains the image data part (e.g., 'data:image/jpeg;base64,')
            if (!base64Image.StartsWith("data:image"))
            {
                throw new ArgumentException("Invalid Base64 image format.");
            }

            // Remove the prefix (data:image/png;base64, or similar) if present
            var base64Data = base64Image.Substring(base64Image.IndexOf(",") + 1);

            // Convert Base64 string to byte array
            byte[] imageBytes = Convert.FromBase64String(base64Data);

            // Create a MemoryStream from the byte array
            using var stream = new MemoryStream(imageBytes);

            // Prepare the upload parameters for Cloudinary
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription("image", stream),  // Name can be generic, Cloudinary handles the file name
                PublicId = $"flower_imgs/{Guid.NewGuid()}"  // Optionally, generate a unique ID for Cloudinary
            };

            // Upload the image to Cloudinary
            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            // Check for errors during upload
            if (uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception($"Image upload failed: {uploadResult.Error?.Message}");
            }

            // Return the URL of the uploaded image
            return uploadResult.SecureUri?.ToString();
        }



    }
}
