using System;

namespace QuanLyTiecCuoi.DataTransferObject
{
    public class THUCDONDTO
    {
        public int MaPhieuDat { get; set; }
        public int MaMonAn { get; set; }
        public int? SoLuong { get; set; }
        public decimal? DonGia { get; set; }
        public string GhiChu { get; set; }

        // Navigation properties
        public MONANDTO MonAn { get; set; }
        public PHIEUDATTIECDTO PhieuDatTiec { get; set; }
    }
}