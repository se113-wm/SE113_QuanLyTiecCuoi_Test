using System;
using System.Collections.Generic;

namespace QuanLyTiecCuoi.DataTransferObject
{
    public class HallDTO
    {
        public int HallId { get; set; }
        public int? HallTypeId { get; set; }
        public string HallName { get; set; }
        public int? MaxTableCount { get; set; }
        public string Note { get; set; }

        // Navigation properties
        public HallTypeDTO HallType { get; set; }
    }
}