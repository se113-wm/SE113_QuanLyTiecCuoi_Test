using System;

namespace QuanLyTiecCuoi.DataTransferObject
{
    public class ShiftDTO
    {
        public int ShiftId { get; set; }
        public string ShiftName { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
    }
}