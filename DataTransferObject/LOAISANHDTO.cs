using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyTiecCuoi.DataTransferObject
{
    public class LOAISANHDTO
    {
        public int MaLoaiSanh { get; set; }
        public string TenLoaiSanh { get; set; }
        public decimal? DonGiaBanToiThieu { get; set; }
    }
}
