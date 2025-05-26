using System;

namespace QuanLyTiecCuoi.DataTransferObject
{
    public class CHITIETDVDTO
    {
        public int MaPhieuDat { get; set; }
        public int MaDichVu { get; set; }
        public int? SoLuong { get; set; }
        public decimal? DonGia { get; set; }
        public decimal? ThanhTien { get; set; }
        public string GhiChu { get; set; }

        // Navigation properties
        public DICHVUDTO DichVu { get; set; }
        public PHIEUDATTIECDTO PhieuDatTiec { get; set; }
    }
}