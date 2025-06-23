using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace QuanLyTiecCuoi.DataTransferObject
{
    public class PHIEUDATTIECDTO
    {
        public int MaPhieuDat { get; set; }
        public string TenChuRe { get; set; }
        public string TenCoDau { get; set; }
        public string DienThoai { get; set; }
        public DateTime? NgayDatTiec { get; set; }
        public DateTime? NgayDaiTiec { get; set; }
        public int? MaCa { get; set; }
        public int? MaSanh { get; set; }
        public decimal? TienDatCoc { get; set; }
        public int? SoLuongBan { get; set; }
        public int? SoBanDuTru { get; set; }
        public DateTime? NgayThanhToan { get; set; }
        public decimal? DonGiaBanTiec { get; set; }
        public decimal? TongTienBan { get; set; }
        public decimal? TongTienDV { get; set; }
        public decimal? TongTienHoaDon { get; set; }
        public decimal? TienConLai { get; set; }
        public decimal? ChiPhiPhatSinh { get; set; }
        public decimal? TienPhat { get; set; }

        // Navigation properties
        public CADTO Ca { get; set; }
        public SANHDTO Sanh { get; set; }

        public string TrangThai
        {
            get
            {
                if (NgayThanhToan != null)
                    return "Đã thanh toán";
                if (NgayDaiTiec == null)
                    return "";
                var ngayDaiTiecDate = NgayDaiTiec.Value.Date;
                var today = DateTime.Now.Date;
                if (ngayDaiTiecDate > today)
                    return "Chưa tổ chức";
                if (ngayDaiTiecDate == today)
                    return "Chưa thanh toán";
                if (ngayDaiTiecDate < today)
                    return "Trễ thanh toán";
                return "";
            }
        }

        public Brush TrangThaiBrush
        {
            get
            {
                if (NgayThanhToan != null)
                    return Brushes.Green;
                if (NgayDaiTiec == null)
                    return Brushes.Black;
                var ngayDaiTiecDate = NgayDaiTiec.Value.Date;
                var today = DateTime.Now.Date;
                if (ngayDaiTiecDate > today)
                    return Brushes.Blue;
                if (ngayDaiTiecDate == today)
                    return Brushes.Orange;
                if (ngayDaiTiecDate < today)
                    return Brushes.Red;
                return Brushes.Black;
            }
        }
    }
}