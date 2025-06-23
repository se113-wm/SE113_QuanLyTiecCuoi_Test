using System;

namespace QuanLyTiecCuoi.DataTransferObject
{
    public class CTBAOCAODDTO
    {
        //add STT
        public int STT { get; set; }
        public int Ngay { get; set; }
        public int Thang { get; set; }
        public int Nam { get; set; }
        public int? SoLuongTiec { get; set; }
        public decimal? DoanhThu { get; set; }
        public decimal? TiLe { get; set; }
        public string NgayHienThi => $"{Ngay:D2}/{Thang:D2}/{Nam}";
        // Navigation property (optional, similar to SANHDTO)
        public BAOCAODDTO BaoCaoD { get; set; }
    }
}