using System.Collections.Generic;
using QuanLyTiecCuoi.Model;

namespace QuanLyTiecCuoi.DataAccessLayer.IRepository
{
    public interface ILoaiSanhRepository
    {
        IEnumerable<LOAISANH> GetAll();
        LOAISANH GetById(int maLoaiSanh);
        void Create(LOAISANH loaiSanh);
        void Update(LOAISANH loaiSanh);
        void Delete(int maLoaiSanh);
    }
}