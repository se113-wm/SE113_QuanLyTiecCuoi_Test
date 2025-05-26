using System.Collections.Generic;
using QuanLyTiecCuoi.Model;

namespace QuanLyTiecCuoi.DataAccessLayer.IRepository
{
    public interface INhomNguoiDungRepository
    {
        IEnumerable<NHOMNGUOIDUNG> GetAll();
        NHOMNGUOIDUNG GetById(string maNhomNguoiDung);
        void Create(NHOMNGUOIDUNG nhomNguoiDung);
        void Update(NHOMNGUOIDUNG nhomNguoiDung);
        void Delete(string maNhomNguoiDung);
    }
}