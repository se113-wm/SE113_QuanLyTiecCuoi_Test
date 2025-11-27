using System;

namespace QuanLyTiecCuoi.DataTransferObject
{
    public class RevenueReportDetailDTO
    {
        public int RowNumber { get; set; }
        public int Day { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int? WeddingCount { get; set; }
        public decimal? Revenue { get; set; }
        public decimal? Ratio { get; set; }
        public string DisplayDate => $"{Day:D2}/{Month:D2}/{Year}";
        
        // Navigation property
        public RevenueReportDTO RevenueReport { get; set; }
    }
}