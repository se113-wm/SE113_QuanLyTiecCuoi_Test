using System.Collections.Generic;
using QuanLyTiecCuoi.Model;

namespace QuanLyTiecCuoi.DataAccessLayer.IRepository
{
    public interface IHallTypeRepository
    {
        IEnumerable<HallType> GetAll();
        HallType GetById(int hallTypeId);
        void Create(HallType hallType);
        void Update(HallType hallType);
        void Delete(int hallTypeId);
    }
}