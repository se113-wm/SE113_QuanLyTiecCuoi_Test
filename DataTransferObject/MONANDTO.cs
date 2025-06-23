using QuanLyTiecCuoi.BusinessLogicLayer.Helpers;
using System.IO;

namespace QuanLyTiecCuoi.DataTransferObject
{
    public class MONANDTO
    {
        public int MaMonAn { get; set; }
        public string TenMonAn { get; set; }
        public decimal? DonGia { get; set; }
        public string GhiChu { get; set; }

        public string ImagePath
        {
            get
            {
                var folder = Path.Combine(ImageHelper.BaseImagePath, "Food");
                var path = Path.Combine(folder, MaMonAn + ".jpg");
                return File.Exists(path) ? path : null;
            }
        }
    }
}