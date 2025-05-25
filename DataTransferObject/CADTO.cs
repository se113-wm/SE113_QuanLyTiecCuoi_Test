using System;

namespace QuanLyTiecCuoi.DataTransferObject
{
    public class CADTO
    {
        public int MaCa { get; set; }
        public string TenCa { get; set; }
        public TimeSpan? ThoiGianBatDauCa { get; set; }
        public TimeSpan? ThoiGianKetThucCa { get; set; }
    }
}