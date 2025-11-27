using QuanLyTiecCuoi.BusinessLogicLayer.Helpers;
using System.IO;

namespace QuanLyTiecCuoi.DataTransferObject
{
    public class DishDTO
    {
        public int DishId { get; set; }
        public string DishName { get; set; }
        public decimal? UnitPrice { get; set; }
        public string Note { get; set; }

        public string ImagePath
        {
            get
            {
                var folder = Path.Combine(ImageHelper.BaseImagePath, "Food");
                var path = Path.Combine(folder, DishId + ".jpg");
                return File.Exists(path) ? path : null;
            }
        }
    }
}