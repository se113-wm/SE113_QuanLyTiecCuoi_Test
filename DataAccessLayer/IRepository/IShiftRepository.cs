using System.Collections.Generic;
using QuanLyTiecCuoi.Model;

namespace QuanLyTiecCuoi.DataAccessLayer.IRepository
{
    public interface IShiftRepository
    {
        IEnumerable<Shift> GetAll();
        Shift GetById(int shiftId);
        void Create(Shift shift);
        void Update(Shift shift);
        void Delete(int shiftId);
    }
}