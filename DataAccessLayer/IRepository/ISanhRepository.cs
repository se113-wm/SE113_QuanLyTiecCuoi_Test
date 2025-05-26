using System.Collections.Generic;
using QuanLyTiecCuoi.Model;

namespace QuanLyTiecCuoi.DataAccessLayer.IRepository
{
    public interface ISanhRepository
    {
        IEnumerable<SANH> GetAll();
        SANH GetById(int maSanh);
        void Create(SANH sanh);
        void Update(SANH sanh);
        void Delete(int maSanh);
    }
}