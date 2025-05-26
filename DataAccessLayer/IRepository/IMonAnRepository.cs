using System.Collections.Generic;
using QuanLyTiecCuoi.Model;

namespace QuanLyTiecCuoi.DataAccessLayer.IRepository
{
    public interface IMonAnRepository
    {
        IEnumerable<MONAN> GetAll();
        MONAN GetById(int maMonAn);
        void Create(MONAN monAn);
        void Update(MONAN monAn);
        void Delete(int maMonAn);
    }
}