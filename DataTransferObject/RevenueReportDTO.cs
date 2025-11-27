using System;
using System.Collections.Generic;

namespace QuanLyTiecCuoi.DataTransferObject
{
    public class RevenueReportDTO
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal? TotalRevenue { get; set; }

        // Navigation property
        public ICollection<RevenueReportDetailDTO> RevenueReportDetails { get; set; }
    }
}