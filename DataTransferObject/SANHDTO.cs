using System;
using System.Collections.Generic;

namespace QuanLyTiecCuoi.DataTransferObject
{
    public class SANHDTO
    {
        public int MaSanh { get; set; }
        public int? MaLoaiSanh { get; set; }
        public string TenSanh { get; set; }
        public int? SoLuongBanToiDa { get; set; }
        public string GhiChu { get; set; }

        // Navigation properties
        public LOAISANHDTO LoaiSanh { get; set; }
    }
}