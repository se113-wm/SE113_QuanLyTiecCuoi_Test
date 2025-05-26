using System.Collections.Generic;
using QuanLyTiecCuoi.Model;

namespace QuanLyTiecCuoi.DataAccessLayer.IRepository
{
    public interface INguoiDungRepository
    {
        IEnumerable<NGUOIDUNG> GetAll();
        NGUOIDUNG GetById(int maNguoiDung);
        void Create(NGUOIDUNG nguoiDung);
        void Update(NGUOIDUNG nguoiDung);
        void Delete(int maNguoiDung);
    }
}