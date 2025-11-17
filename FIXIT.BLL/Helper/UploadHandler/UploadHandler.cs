using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.BLL.Helper.UploadHandler
{
    public class UploadHandler
    {
        private readonly string _webRootPath;

        public UploadHandler(string webRootPath)
        {
            _webRootPath = webRootPath;
        }

        public string Upload(IFormFile file, string folder = "ProfilePics", string? oldFilePath = null)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("No file uploaded.");

            List<string> validExtensions = new() { ".jpg", ".png", ".gif", ".jpeg" };
            string extension = Path.GetExtension(file.FileName).ToLower();

            if (!validExtensions.Contains(extension))
                throw new InvalidOperationException($"Invalid extension ({string.Join(", ", validExtensions)})");

            if (file.Length > 5 * 1024 * 1024)
                throw new InvalidOperationException("Maximum size can be 5 MB.");

            string fileName = $"{Guid.NewGuid()}{extension}";
            string uploadPath = Path.Combine(_webRootPath, "images", folder);

            Directory.CreateDirectory(uploadPath);

            string fullPath = Path.Combine(uploadPath, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                 file.CopyToAsync(stream);
            }

            if (!string.IsNullOrEmpty(oldFilePath))
            {
                string oldFullPath = Path.Combine(_webRootPath, oldFilePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                if (File.Exists(oldFullPath))
                    File.Delete(oldFullPath);
            }

            return $"/images/{folder}/{fileName}";
        }

       
    }


}
