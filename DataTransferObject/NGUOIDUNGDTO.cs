using System;

namespace QuanLyTiecCuoi.DataTransferObject
{
    public class NGUOIDUNGDTO
    {
        public int MaNguoiDung { get; set; }
        public string TenDangNhap { get; set; }
        public string MatKhauHash { get; set; }
        public string HoTen { get; set; }
        public string Email { get; set; }
        public string MaNhom { get; set; }

        // Navigation property
        public NHOMNGUOIDUNGDTO NhomNguoiDung { get; set; }
    }
}