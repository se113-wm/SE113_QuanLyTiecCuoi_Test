using System.Collections.Generic;
using QuanLyTiecCuoi.Model;

namespace QuanLyTiecCuoi.DataAccessLayer.IRepository
{
    public interface IHallRepository
    {
        IEnumerable<Hall> GetAll();
        Hall GetById(int hallId);
        void Create(Hall hall);
        void Update(Hall hall);
        void Delete(int hallId);
    }
}