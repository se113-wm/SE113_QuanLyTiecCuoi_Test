using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyTiecCuoi.DataTransferObject
{
    public class HallTypeDTO
    {
        public int HallTypeId { get; set; }
        public string HallTypeName { get; set; }
        public decimal? MinTablePrice { get; set; }
    }
}
