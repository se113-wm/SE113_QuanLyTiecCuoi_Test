using QuanLyTiecCuoi.Model;
using System;
using QuanLyTiecCuoi.BusinessLogicLayer.Helpers;
using System.IO;

namespace QuanLyTiecCuoi.DataTransferObject
{
    public class DICHVUDTO
    {
        public int MaDichVu { get; set; }
        public string TenDichVu { get; set; }
        public decimal? DonGia { get; set; }
        public string GhiChu { get; set; }

        public string ImagePath
        {
            get
            {
                var folder = Path.Combine(ImageHelper.BaseImagePath, "Service");
                var path = Path.Combine(folder, MaDichVu + ".jpg");
                return File.Exists(path) ? path : null;
            }
        }
    }
}