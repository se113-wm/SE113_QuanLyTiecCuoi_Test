using System;

namespace QuanLyTiecCuoi.Tests.Helpers
{
    /// <summary>
    /// Các hàm hỗ trợ cho việc test
    /// </summary>
    public static class TestHelper
    {
        /// <summary>
        /// Tạo chuỗi ngẫu nhiên cho test data
        /// </summary>
        public static string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            var result = new char[length];

            for (int i = 0; i < length; i++)
            {
                result[i] = chars[random.Next(chars.Length)];
            }

            return new string(result);
        }

        /// <summary>
        /// Tạo ngày ngẫu nhiên trong tương lai (cho booking date)
        /// </summary>
        public static DateTime GenerateFutureDate(int minDays = 1, int maxDays = 365)
        {
            var random = new Random();
            return DateTime.Now.AddDays(random.Next(minDays, maxDays));
        }

        /// <summary>
        /// Đường dẫn đến file exe của ứng dụng (cho UI test)
        /// </summary>
        public static string GetApplicationPath()
        {
            // Điều chỉnh path này theo cấu trúc project của bạn
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            return System.IO.Path.Combine(basePath, @"..\..\..\QuanLyTiecCuoi\bin\Debug\QuanLyTiecCuoi.exe");
        }
    }
}