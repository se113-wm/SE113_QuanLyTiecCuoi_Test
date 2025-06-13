using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace QuanLyTiecCuoi.BusinessLogicLayer.Helpers
{
    public static class ImageHelper
    {
        public static string BaseImagePath => Path.Combine(ProjectPaths.GetProjectRoot(), "Images");

        public static string GetImagePath(string loai, string id)
        {
            var folder = Path.Combine(BaseImagePath, loai);
            var extensions = new[] { ".jpg", ".png", ".jpeg" };

            foreach (var ext in extensions)
            {
                var path = Path.Combine(folder, id + ext);
                if (File.Exists(path))
                    return path;
            }

            return Path.Combine(BaseImagePath, "default.jpg");
        }

        public static void SaveImage(string loai, string id, string sourceFilePath)
        {
            var folder = Path.Combine(BaseImagePath, loai);
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

            var extension = Path.GetExtension(sourceFilePath);
            var destPath = Path.Combine(folder, id + extension);

            // Nếu file đã tồn tại, cố gắng xóa trước khi copy
            if (File.Exists(destPath))
            {
                try
                {
                    File.Delete(destPath);
                }
                catch (IOException ex)
                {
                    // File đang bị process khác giữ, không thể xóa
                    throw new IOException("Không thể ghi đè file ảnh vì file đang được sử dụng bởi ứng dụng khác.", ex);
                }
            }

            File.Copy(sourceFilePath, destPath, true);
        }
        
        public static bool IsEditCacheImageSameAsCurrent(int id, string loai)
        {
            string folder = Path.Combine(ImageHelper.BaseImagePath, loai);
            string cachePath = Path.Combine(folder, "Editcache.jpg");
            string currentPath = Path.Combine(folder, id + ".jpg");
            if (!File.Exists(cachePath) && !File.Exists(currentPath))
            {
                //MessageBox.Show("cả 2 đều không tồn tại");
                return true; // Both don't exist, considered same
            }
            //MessageBox.Show("có 1 trong 2 tổn tại");
            if (File.Exists(cachePath) && !File.Exists(currentPath))
            {
                //MessageBox.Show("cache tồn tại, current không tồn tại");
                return false; // Cache exists, current doesn't
            }

            if (!File.Exists(cachePath) && File.Exists(currentPath))
            {
                //MessageBox.Show("cache không tồn tại, current tồn tại");
                return true;
            }

            // Both exist, compare bytes
            byte[] cacheBytes = File.ReadAllBytes(cachePath);
            byte[] currentBytes = File.ReadAllBytes(currentPath);
            if (cacheBytes.Length != currentBytes.Length)
                return false;
            for (int i = 0; i < cacheBytes.Length; i++)
                if (cacheBytes[i] != currentBytes[i])
                    return false;
            return true;
        }
    }

    public static class ProjectPaths
    {
        public static string GetProjectRoot()
        {
            // Lùi 2 cấp từ bin\Debug\netX.X → về thư mục gốc dự án
            return Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)
                            .Parent?
                            .Parent?
                            .Parent?
                            .FullName ?? "";
        }
    }

}
