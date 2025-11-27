using QuanLyTiecCuoi.Model;
using System;
using QuanLyTiecCuoi.BusinessLogicLayer.Helpers;
using System.IO;

namespace QuanLyTiecCuoi.DataTransferObject
{
    public class ServiceDTO
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public decimal? UnitPrice { get; set; }
        public string Note { get; set; }

        public string ImagePath
        {
            get
            {
                var folder = Path.Combine(ImageHelper.BaseImagePath, "Service");
                var path = Path.Combine(folder, ServiceId + ".jpg");
                return File.Exists(path) ? path : null;
            }
        }
    }
}