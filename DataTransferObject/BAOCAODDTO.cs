using System;
using System.Collections.Generic;

namespace QuanLyTiecCuoi.DataTransferObject
{
    public class BAOCAODDTO
    {
        public int Thang { get; set; }
        public int Nam { get; set; }
        public decimal? TongDoanhThu { get; set; }

        // Navigation property: Danh sách chi tiết báo cáo doanh thu
        public ICollection<CTBAOCAODDTO> CTBAOCAODS { get; set; }
    }
}