using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Web;

namespace APIPeliculas.Utils
{
    public class FirebaseStorage : IFirebaseStorage
    {
        private readonly string _bucketName;
        private readonly StorageClient _storageClient;
        public FirebaseStorage(IConfiguration configuration, StorageClient storageClient)
        {
            _bucketName = configuration["FirebaseBucket:URL"];
            _storageClient = storageClient;
        }

        public async Task<string> SaveFile(IFormFile file, string folderName)
        {
            if (file == null || file.Length == 0)
            {
                return "";
            }

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var fileExtension = Path.GetExtension(file.FileName);
            var filePath = $"{folderName}/{fileName}";            
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                stream.Position = 0;
                await _storageClient.UploadObjectAsync(_bucketName, filePath, $"image/{fileExtension.Substring(1)}", stream);
            }

            return $"https://firebasestorage.googleapis.com/v0/b/{_bucketName}/o/{folderName}%2F{Uri.EscapeDataString(fileName)}?alt=media";
        }

        public async Task DeleteFile(string route, string folderName)
        {
            try
            {
                if (string.IsNullOrEmpty(route)) return;

                string decodedUrl = HttpUtility.UrlDecode(route);

                string path = new Uri(decodedUrl).AbsolutePath;

                var fileName = Path.GetFileName(path);
                var filePath = $"{folderName}/{fileName}";
                await _storageClient.DeleteObjectAsync(_bucketName, filePath);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public async Task<string> EditFile(IFormFile file, string route, string folderName)
        {
            await DeleteFile(route, folderName);
            return await SaveFile(file, folderName);
        }
    }
}
