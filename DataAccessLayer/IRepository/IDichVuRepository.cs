using System.Collections.Generic;
using QuanLyTiecCuoi.Model;

namespace QuanLyTiecCuoi.DataAccessLayer.IRepository
{
    public interface IDichVuRepository
    {
        IEnumerable<DICHVU> GetAll();
        DICHVU GetById(int maDichVu);
        void Create(DICHVU dichVu);
        void Update(DICHVU dichVu);
        void Delete(int maDichVu);
    }
}