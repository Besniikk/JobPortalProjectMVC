using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using JobPortalProjectMVC.Helpers;
using JobPortalProjectMVC.Interfaces;
using Microsoft.Extensions.Options;

namespace JobPortalProjectMVC.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly Cloudinary _cloudinary;
        public PhotoService(IOptions<CloudinarySettings> config) //allow to bring cloudinary
        {
            var acc = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
                );
            _cloudinary = new Cloudinary(acc);
        }
        /*public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file) // upload image
        {
            var uploadResult = new ImageUploadResult();
            if (file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face")
                };
                uploadResult = await _cloudinary.UploadAsync(uploadParams);
            }
            return uploadResult;
        }*/

        public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
        {
            try
            {
                var uploadResult = new ImageUploadResult();
                if (file.Length > 0)
                {
                    using var stream = file.OpenReadStream();
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(file.FileName, stream),
                        Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face")
                    };
                    uploadResult = await _cloudinary.UploadAsync(uploadParams);
                }
                return uploadResult;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to upload image to Cloudinary.", ex);
            }
        }

        public async Task<DeletionResult> DeletePhotoAsync(string publicId) //delete image
        {
            var deleteParams = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deleteParams);

            return result;
        }
    }
}

